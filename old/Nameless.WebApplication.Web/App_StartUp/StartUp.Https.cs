﻿using Microsoft.AspNetCore.Builder;

namespace Nameless.WebApplication.Web {
    public partial class StartUp {
        #region Public Methods

        public void ConfigureHttps (IApplicationBuilder app) {
            app.UseHttpsRedirection ();
        }

        #endregion
    }
}