import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { User } from '../models/user';

import { AlertService, AuthenticationService } from '../services/index';

/**
*	This class represents the lazy loaded LoginComponent.
*/

@Component({
	moduleId: module.id,
	selector: 'login-cmp',
	templateUrl: 'login.component.html'
})

export class LoginComponent implements OnInit {
	
	user: User = new User();
	
	loading = false;
 
    constructor(
        private router: Router,
        private authenticationService: AuthenticationService,
        private alertService: AlertService) { }
 
    ngOnInit() {
        // reset login status
        this.authenticationService.logout();
    }
 
    loginUser() {
		//${JSON.stringify(this.user)}
		
        this.loading = true;
        
        this.authenticationService.login(this.user.username, this.user.password)
            .subscribe(
                data => {
                    this.router.navigate(['/dashboard/home']);
                },
                error => {
                    //alert("The user name or password is incorrect.");
                    this.alertService.error("The user name or password is incorrect.");
                    this.loading = false;
                });
    }
}
