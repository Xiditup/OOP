using GalaSoft.MvvmLight.Messaging;
using PhoneService.BLL.Services;
using PhoneService.Commands;
using PhoneService.DAL.Entities;
using PhoneService.Models;
using System.Text.RegularExpressions;

namespace PhoneService.ViewModels
{
    public class ReviewVM : ViewModelBase
    {
        private readonly ReviewManager _reviewManager;
        private readonly Mediator _mediator;

        public string Description { get; set; } = "";

        public Request? SelectedRequest { get; set; } = null;

        public string UserLogin { get; set; } = "";

        private string _mark = "0";
        public string Mark
        {
            get => _mark;
            set
            {
                if (Regex.IsMatch(value, "^[0-5]*$"))
                {
                    try
                    {
                        if (value?.Length == 0)
                        {
                            SetProperty(ref _mark, "0");
                            return;
                        }
                        if (!string.IsNullOrEmpty(value) && value.Length > 1 && value[0] == '0' && char.IsDigit(value[1]))
                        {
                            value = value[1..];
                        }
                        var result = Convert.ToInt32(value);
                        if (result > 5)
                        {
                            if (_mark == "0")
                            {
                                SetProperty(ref _mark, result.ToString()[0].ToString());
                                return;
                            }
                            SetProperty(ref _mark, "5");
                            return;
                        }
                        SetProperty(ref _mark, value);
                    }
                    catch
                    {
                        if (value?.Length == 0)
                        {
                            SetProperty(ref _mark, "0");
                        }
                        else
                        {
                            SetProperty(ref _mark, "5");
                        }
                    }
                }
            }
        }

        public IEnumerable<Review> Reviews { get; set; } = [];
        public IEnumerable<Request> ClientRequests { get; set; } = [];

        public bool CanCreateComment { get; set; } = false;

        private RelayCommand _createReview = null!;
        public RelayCommand CreateReview
        {
            get => _createReview ??= new(
                async () =>
                {
                    if (ViewsVM.StaticUser == null || SelectedRequest == null) return;
                    await _reviewManager.CreateReview(ViewsVM.StaticUser, SelectedRequest, Convert.ToInt32(Mark), Description);
                    Update();
                }, () =>
                {
                    return SelectedRequest != null &&
                           Mark != "";
                });
        }

        private RelayCommand<Review> _changeReview = null!;
        public RelayCommand<Review> ChangeReview
        {
            get => _changeReview ??= new(
                async (review) =>
                {
                    if (review == null) return;
                    if (review.IsEditing)
                    {
                        Messenger.Default.Send(true);
                        review.IsEditing = false;
                        await _reviewManager.ChangeReview(review);
                    }
                    else
                    {
                        Messenger.Default.Send(false);
                        review.IsEditing = true;
                    }
                    Update();
                });
        }

        private RelayCommand<Review> _deleteReview = null!;
        public RelayCommand<Review> DeleteReview
        {
            get => _deleteReview ??= new(
                async (review) =>
                {
                    if (review == null) return;
                    await _reviewManager.DeleteReview(review);
                    Update();
                });
        }

        public ReviewVM(ReviewManager reviewManager, Mediator mediator)
        {
            _reviewManager = reviewManager;
            _mediator = mediator;
            Update();
            _mediator.UpdateRequestsEvent += Update;
            _mediator.UpdateUserEvent += Update;
        }

        public void Update()
        {
            if (ViewsVM.StaticUser != null)
            {
                CanCreateComment = false;
                if (ViewsVM.StaticUser.Role == "Client")
                {
                    CanCreateComment = true;
                }
                ClientRequests = _reviewManager.GetClientRequests(ViewsVM.StaticUser);
                UserLogin = ViewsVM.StaticUser.Login;
            }
            SelectedRequest = null;
            Description = "";
            Mark = "0";
            Reviews = _reviewManager.GetAllReviews().Select(r => { r.IsOwner = ViewsVM.StaticUser?.Id == r.UserId; return r; });
            if (MainWindow.IsRu)
            {
                Reviews = Reviews.Select(r => { r.EditText = !r.IsEditing ? "Изменить" : "Сохранить"; return r; });
            }
            else
            {
                Reviews = Reviews.Select(r => { r.EditText = !r.IsEditing ? "Edit" : "Save"; return r; });
            }
        }
    }
}
