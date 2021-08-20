// <copyright file="HomeController.cs" company="Microsoft">
// Copyright (c) Microsoft. All Rights Reserved.
// </copyright>

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Calypso.TeamsApp123.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BackendController : ControllerBase
    {
        private const string TaskReadWriteScope = "https://graph.microsoft.com/User.Read";
        private const string UserAssertionType = "urn:ietf:params:oauth:grant-type:jwt-bearer";


        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="BackendController"/> class.
        /// </summary>
        /// <param name="configuration">IConfiguration instance.</param>
        /// <param name="httpClientFactory">IHttpClientFactory instance.</param>
        /// <param name="httpContextAccessor">IHttpContextAccessor instance.</param>
        public BackendController(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<string> GetInfo([FromQuery]string token)
        {
            try
            {

                var c = new GraphServiceClient(new DelegateAuthenticationProvider(async (r) =>
                {          
                    r.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }));
                var me = await c.Me.Request().GetAsync();
                var chatMessage = new ChatMessage
                {
                    Body = new ItemBody
                    {
                        Content = $"test"
                    }
                };
                await c.Teams["44faa137-d83d-4bc9-86a4-6f601c139263"].Channels["19:B5-VdfMOwSbBA_Va0YuZE7XakygUc0Z8TSQKbACBY301@thread.tacv2"].Messages
                    .Request()
                    .AddAsync(chatMessage);
                return me.DisplayName;
            }
            catch (System.Exception)
            {

                throw;
            }
          
        }
    }
}
