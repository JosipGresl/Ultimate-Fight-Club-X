using Microsoft.Extensions.Logging;
using Ultimate_Fight_Club_X.Services;
using Ultimate_Fight_Club_X.ViewModels;


namespace Ultimate_Fight_Club_X
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            builder.Services.AddTransient<AddMemberViewModel>();
            builder.Services.AddTransient<AddMemberPage>();
            builder.Services.AddSingleton<MemberDatabase>();
            builder.Services.AddTransient<MembersViewModel>();
            builder.Services.AddTransient<MembersPage>();
            builder.Services.AddTransient<AttendanceViewModel>();
            builder.Services.AddTransient<AttendancePage>();
            builder.Services.AddTransient<FeesViewModel>();
            builder.Services.AddTransient<FeesPage>();
            builder.Services.AddTransient<AddPaymentViewModel>();
            builder.Services.AddTransient<AddPaymentPage>();
            builder.Services.AddTransient<MemberDetailViewModel>();
            builder.Services.AddTransient<MemberDetailPage>();
            builder.Services.AddTransient<DashboardViewModel>();
            builder.Services.AddTransient<DashboardPage>();
            return builder.Build();
        }
    }
}
