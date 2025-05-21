using PhoneService.DAL.Entities;
using PhoneService.DAL.Repository;

namespace PhoneService.BLL.Services
{
    public class ReviewManager
    {
        private readonly ReviewRepository _reviewRepository;
        private readonly RequestRepository _requestRepository;
        public ReviewManager(ReviewRepository reviewRepository, RequestRepository requestRepository)
        {
            _reviewRepository = reviewRepository;
            _requestRepository = requestRepository;
        }

        public IEnumerable<Review> GetAllReviews()
        {
            return _reviewRepository.GetAll();
        }

        public IEnumerable<Request> GetClientRequests(User user)
        {
            return _requestRepository.GetClientClosedRequestsWithoutReview(user.Id);
        }

        public async Task CreateReview(User user, Request request, int mark, string description)
        {
            if (mark < 0 || mark > 5)
            {
                throw new ArgumentException("Оценка должна находиться в диапазоне 0-5");
            }

            var review = new Review
            {
                Mark = mark,
                Description = description,
                UserId = user.Id,
                User = user,
                RequestId = request.Id,
                Request = request
            };

            await _reviewRepository.AddAsync(review);
        }

        public async Task DeleteReview(Review review)
        {
            await _reviewRepository.RemoveAsync(review);
        }

        public async Task ChangeReview(Review review)
        {
            if (review.Mark > 5)
            {
                review.Mark = 5;
            }
            if (review.Mark < 0)
            {
                review.Mark = 0;
            }

            await _reviewRepository.UpdateAsync(review);
        }
    }
}
