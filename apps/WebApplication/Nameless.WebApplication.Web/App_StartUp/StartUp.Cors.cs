﻿using Microsoft.AspNetCore.Builder;

namespace Nameless.WebApplication.Web {
    public partial class StartUp {
        #region Public Methods

        public void UsingCors (IApplicationBuilder app) {
            app.UseCors (policy => {
                policy
                    .AllowAnyOrigin ()
                    .AllowAnyMethod ()
                    .AllowAnyHeader ();
            });
        }

        #endregion
    }
}