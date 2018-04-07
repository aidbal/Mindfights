import { NgModule } from '@angular/core';

import * as ApiServiceProxies from './service-proxies';

@NgModule({
    providers: [
        ApiServiceProxies.RoleServiceProxy,
        ApiServiceProxies.SessionServiceProxy,
        ApiServiceProxies.TenantServiceProxy,
        ApiServiceProxies.UserServiceProxy,
        ApiServiceProxies.TokenAuthServiceProxy,
        ApiServiceProxies.AccountServiceProxy,
        ApiServiceProxies.ConfigurationServiceProxy,
		ApiServiceProxies.MindfightServiceProxy,
		ApiServiceProxies.PlayerServiceProxy,
		ApiServiceProxies.QuestionServiceProxy,
		ApiServiceProxies.RegistrationServiceProxy,
		ApiServiceProxies.ResultServiceProxy,
		ApiServiceProxies.TeamAnswerServiceProxy,
		ApiServiceProxies.TeamServiceProxy,
		ApiServiceProxies.TourServiceProxy
    ]
})
export class ServiceProxyModule { }