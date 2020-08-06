using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SyncMusicFromExternalSources.Areas.Identity;
using SyncMusicFromExternalSources.Data;
using System.Net.Http;
using SpotifyApi.NetCore;
using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.Linq;
using System;
using SyncMusicFromExternalSources.Shared;

namespace SyncMusicFromExternalSources
{
    public class Startup
    {
        public static string SpotifyUserAccessToken;
        public static string GoogleUserAccessToken;
        public static string TwitterUserAccessToken;
        public static string FacebookUserAccessToken;
        public static string GoogleApisApplicationName = "Put your Google App Name in this string";
        public static string GoogleApisApiKey = "Put your Google API Key in this string";
        public static long DivIndex = 0;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();

            services.AddAuthentication().AddSpotify(options =>
            {
                options.ClientId = "Put your Spotify API Client ID in this string";
                options.ClientSecret = "Put your Spotify API Client Secret in this string";
                options.CallbackPath = "/SpotifyAPI/spotifylistplaylists";
                options.SaveTokens = true;

                //You do need all these scopes but this is for demonstration purposes of all possibilities.
                options.Scope.Add("user-read-playback-position");
                options.Scope.Add("user-read-email");
                options.Scope.Add("user-library-read");
                options.Scope.Add("user-top-read");
                options.Scope.Add("playlist-modify-public");
                options.Scope.Add("user-follow-read");
                options.Scope.Add("user-read-playback-state");
                options.Scope.Add("user-modify-playback-state");
                options.Scope.Add("user-read-private");
                options.Scope.Add("playlist-read-private");
                options.Scope.Add("user-library-modify");
                options.Scope.Add("playlist-read-collaborative");
                options.Scope.Add("playlist-modify-private");
                options.Scope.Add("user-follow-modify");
                options.Scope.Add("user-read-currently-playing");
                options.Scope.Add("user-read-recently-played");

                options.Events.OnRemoteFailure = (context) =>
                {
                    return Task.CompletedTask;
                };

                options.Events.OnCreatingTicket = ctx =>
                {
                    List<AuthenticationToken> tokens = ctx.Properties.GetTokens().ToList();

                    tokens.Add(new AuthenticationToken()
                    {
                        Name = "TicketCreated",
                        Value = DateTime.UtcNow.ToString()
                    });

                    ctx.Properties.StoreTokens(tokens);

                    SpotifyUserAccessToken = ctx.Properties.GetTokenValue("access_token");
                    NavMenu.IsLoggedIntoSpotify = true;

                    return Task.CompletedTask;
                };
            }).AddGoogle(options =>
            {
                options.ClientId = "Put your Google Youtube API Client ID in this string";
                options.ClientSecret = "Put your Google Youtube API Client Secret in this string";
                options.CallbackPath = "/YoutubeAPI/youtubelistplaylists";
                options.SaveTokens = true;

                options.Scope.Add("https://www.googleapis.com/auth/youtube.readonly");

                options.Events.OnRemoteFailure = (context) =>
                {
                    return Task.CompletedTask;
                };

                options.Events.OnCreatingTicket = ctx =>
                {
                    List<AuthenticationToken> tokens = ctx.Properties.GetTokens().ToList();

                    tokens.Add(new AuthenticationToken()
                    {
                        Name = "TicketCreated",
                        Value = DateTime.UtcNow.ToString()
                    });

                    ctx.Properties.StoreTokens(tokens);

                    GoogleUserAccessToken = ctx.Properties.GetTokenValue("access_token");
                    NavMenu.IsLoggedIntoGoogle = true;

                    return Task.CompletedTask;
                };
            });

            services.AddSingleton(new HttpClient());
            services.AddSingleton(typeof(IPlaylistsApi), typeof(PlaylistsApi));
            services.AddSingleton(typeof(IArtistsApi), typeof(ArtistsApi));
            services.AddSingleton(typeof(IUsersProfileApi), typeof(UsersProfileApi));
            services.AddSingleton(typeof(IFollowApi), typeof(FollowApi));
            services.AddSingleton(typeof(ISearchApi), typeof(SearchApi));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
