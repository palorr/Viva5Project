import { Injectable } from '@angular/core';
import { Http, Headers, Response, RequestOptions } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map'
 
@Injectable()
export class UserService {
    constructor(private http: Http) { }
 
    registerUser(username: string, password: string, confirmPassword: string) {
        
        let registerURL = 'http://localhost:52686/api/account/register';
        
        let headers = new Headers({ 
            'Content-Type': 'application/json'
        });
        
        let postRequestBody = JSON.stringify({ 
			username: username, 
            password: password,
			confirmPassword: confirmPassword
		});
        
        let options = new RequestOptions({ headers: headers });
        
        return this.http.post(registerURL, postRequestBody, options)
            .map((response: Response) => {
                //console.log('Register Server Response: ', response);
            });
    }
 
}