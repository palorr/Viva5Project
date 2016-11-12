import { Component } from '@angular/core';
import { Router } from '@angular/router';

import { AlertService, UserService } from '../services/index';

import { User } from '../models/user';

/**
*	This class represents the lazy loaded SignupComponent.
*/

@Component({
	moduleId: module.id,
	selector: 'signup-cmp',
	templateUrl: 'signup.component.html'
})

export class SignupComponent {
	
	user: User = new User();
	
	loading = false;
 
    constructor(
        private router: Router,
        private alertService: AlertService,
        private userService: UserService
    ) { }
 
    registerUser() {
        this.loading = true;
		
		if(this.user.password !== this.user.repeatPassword) {
            this.alertService.error("Provided passwords do not match.");
			this.loading = false;
			return;
		}
		
		//alert(`registered!!! ${JSON.stringify(this.user)}`);
		
        this.userService.registerUser(this.user.username, this.user.password, this.user.repeatPassword)
            .subscribe(
                data => {
                    // set success message and pass true paramater to persist the message after redirecting to the login page
                    this.alertService.success('Registration successful', true);
                    this.router.navigate(['/login']);
                },
                error => {
                    this.alertService.error(error);
                    this.loading = false;
                });
                
    }
}
