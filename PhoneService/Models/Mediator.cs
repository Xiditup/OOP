namespace PhoneService.Models
{
    public class Mediator
    {
        public event Action UpdateServicesEvent;
        public event Action UpdateRequestsEvent;
        public event Action UpdateUserEvent;
        public event Action UpdateDetailsEvent;
        public void UpdateServices()
        {
            UpdateServicesEvent?.Invoke();
        }

        public void UpdateRequests()
        {
            UpdateRequestsEvent?.Invoke();
        }

        public void UpdateUser()
        {
            UpdateUserEvent?.Invoke();
        }

        public void UpdateDetails()
        {
            UpdateDetailsEvent?.Invoke();
        }
    }
}
