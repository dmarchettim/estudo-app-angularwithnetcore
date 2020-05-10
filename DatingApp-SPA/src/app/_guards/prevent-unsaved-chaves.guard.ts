import { Injectable } from '@angular/core';
import { CanDeactivate } from '@angular/router';
import { ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router'
import { Observable } from 'rxjs';

import { MemberEditComponent } from '../member/member-edit/member-edit.component';

@Injectable({
    providedIn: 'root'
})
export class PreventUnsavedChanges implements CanDeactivate<MemberEditComponent>{

    canDeactivate(component: MemberEditComponent, currentRoute: ActivatedRouteSnapshot, currentState: RouterStateSnapshot, nextState?: RouterStateSnapshot): boolean | Observable<boolean> | Promise<boolean> {
        
        if(component.editForm.dirty){
            return confirm('Are you sure you want to continue? Any unsaved changes will be lost!');
        }

        return true;
    }

}