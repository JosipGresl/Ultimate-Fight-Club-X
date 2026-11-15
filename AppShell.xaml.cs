namespace Ultimate_Fight_Club_X
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("addmember", typeof(AddMemberPage));
            Routing.RegisterRoute("addpayment", typeof(AddPaymentPage));
        }
    }
}
