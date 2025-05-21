using PhoneService.DAL.Entities;
using PhoneService.DAL.Models;
using PhoneService.DAL.Repository;
using System.Collections;

namespace PhoneService.BLL.Services
{
    public class RequestManager
    {
        private readonly RequestRepository _requestRepository;
        private readonly ServiceRepository _serviceRepository;
        private readonly DetailRepository _detailRepository;
        private readonly UsedDetailRepository _usedDetailRepository;
        private readonly ImageService _imageService;
        private readonly EmailSender _emailSender;
        public RequestManager(
            RequestRepository requestRepository,
            ServiceRepository serviceRepository,
            DetailRepository detailRepository,
            UsedDetailRepository usedDetailRepository,
            ImageService imageService,
            EmailSender emailSender
        )
        {
            _requestRepository = requestRepository;
            _serviceRepository = serviceRepository;
            _detailRepository = detailRepository;
            _usedDetailRepository = usedDetailRepository;
            _imageService = imageService;
            _emailSender = emailSender;
        }

        public IEnumerable<Service> GetAllServices()
        {
            return _serviceRepository.GetAll();
        }

        public IEnumerable<Detail> GetAllDetails()
        {
            return _detailRepository.GetAll();
        }

        public async Task<Request> CreateRequest(
            string name,
            string description,
            string device,
            IEnumerable<FileInfo> images,
            Service service,
            User user
        )
        {
            Request request = new()
            {
                Name = name,
                Description = description,
                Device = device,
                Status = RequestStatus.Created,
                Cost = service.PriceWithDiscount,
                Response = "",
                ServiceId = service.Id,
                Service = service,
                ClientId = user.Id,
                Client = user,
            };

            if (!images.Any())
            {
                throw new ArgumentException("Заявка должна иметь хотя бы 1 фото");
            }

            request.ImagePathes = _imageService.UploadMany(images).ToList();

            ModelValidator.ValidateRequest(request);

            await _requestRepository.AddAsync(request);
            return request;
        }

        public IEnumerable<Request> GetUserRequests(User user)
        {
            if (user.Role == "Client")
            {
                return _requestRepository.GetClientRequests(user.Id);
            }
            return _requestRepository.GetAll();
        }

        public async Task CancelRequestAsync(Request request, bool byMaster = false)
        {
            if (request.Status >= RequestStatus.Approved && !byMaster)
            {
                request.CancelRequired = true;
                //throw new ArgumentException("Нельзя отменить заявку, которая была принята");
            } else
            {
                request.Status = RequestStatus.Canceled;
            }

            await _requestRepository.UpdateAsync(request);
        }

        public IEnumerable<Service> GetServicesByCategory(ServiceCategory serviceCategory)
        {
            return _serviceRepository.GetServicesByNameAndCategory("", serviceCategory);
        }

        public async Task<IEnumerable<Request>> GetRequestsByFiltersAsync(
            User user,
            string requestName,
            string deviceName,
            Service? selectedService,
            RequestStatus selectedStatus,
            bool onlyActive,
            bool onlyWithoutMaster,
            bool onlyCancelRequired
        )
        {
            return await _requestRepository.GetAllFiltered(
                user,
                requestName,
                deviceName,
                selectedService,
                selectedStatus,
                onlyActive,
                onlyWithoutMaster,
                onlyCancelRequired
            );
        }

        public async Task AddDetailsToRequestAsync(Request request, Detail detail, int quantity)
        {
            if (detail.Quantity - quantity < 0)
            {
                throw new ArgumentException("Нельзя потратить больше деталей, чем есть");
            }

            detail.Quantity -= quantity;

            var usedDetail = await _usedDetailRepository.GetByNameForRequestAsync(detail.Name, request.Id);
            if (usedDetail == null)
            {
                usedDetail = new UsedDetail
                {
                    Name = detail.Name,
                    Quantity = quantity,
                    RequestId = request.Id,
                    Request = request
                };
                request.UsedDetails.Add(usedDetail);
                await _requestRepository.UpdateAsync(request);
            }
            else
            {
                usedDetail.Quantity += quantity;
                await _usedDetailRepository.UpdateAsync(usedDetail);
            }

            await _detailRepository.UpdateAsync(detail);
        }

        public Request GetFullRequest(Request request)
        {
            return _requestRepository.GetFullRequestById(request.Id);
        }

        public async Task SaveRequestAsync(Request request, RequestStatus newStatus, string response, int newPrice)
        {
            if (newStatus < request.Status)
            {
                throw new ArgumentException("Новый статус не может быть меньше предыдущего");
            }

            request.Status = newStatus;
            request.Response = response;
            request.Cost = newPrice;
            await _requestRepository.UpdateAsync(request);

            var statusString = "Создан";
            switch (newStatus)
            {
                case RequestStatus.Approved:
                    statusString = "Одобрен";
                    break;
                case RequestStatus.InProgress:
                    statusString = "В процессе";
                    break;
                case RequestStatus.Completed:
                    statusString = "Готов к выдаче";
                    break;
                case RequestStatus.Closed:
                    statusString = "Закрыт";
                    break;
            }

            _ = Task.Run(async () =>
            {
                await _emailSender.SendEmailAsync(
                    request.Client.Email,
                    $"Заявка {request.Name}",
                    $"""
                   Данные вашей заявки были обновлены:
                   Статус: {statusString},
                   Цена: {newPrice},
                   Сообщение от сервиса: {response}
                """
                );
            });
        }

        public async Task SetRequestMasterAsync(Request request, User master)
        {
            request.Master = master;
            request.MasterId = master.Id;

            request.Status = RequestStatus.Approved;

            await _requestRepository.UpdateAsync(request);

            _ = Task.Run(async () =>
                {
                    await _emailSender.SendEmailAsync(
                    request.Client.Email,
                    $"Заявка {request.Name}",
                    $"Мастер {master.Name} взялся за вашу заявку!"
                );
            });
        }
    }
}
