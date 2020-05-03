import { Injectable } from '@angular/core';
import * as alertify from 'alertifyjs';

@Injectable({
  providedIn: 'root'
})
export class AlertifyService {

  constructor() { }

  confirm(message: string, OkCallBack: () => any)
  {
    alertify.confirm(message, (e: any) => 
    {
      if(e)
      {
        OkCallBack();
      } else {};
    })
  }

  success(message: string)
  {
    alertify.success(message);
  }

  error(message: string)
  {
    alertify.error(message);
  }

  warning(message: string)
  {
    alertify.warning(message);
  }

  message(message: string)
  {
    alertify.message(message);
  }
}
