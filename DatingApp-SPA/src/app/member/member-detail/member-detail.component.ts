import { Component, OnInit } from '@angular/core';
import { NgxGalleryAnimation, NgxGalleryOptions, NgxGalleryImage } from 'ngx-gallery';
import { ActivatedRoute, Router, ActivatedRouteSnapshot } from '@angular/router';
import { Route } from '@angular/compiler/src/core';

import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { User } from 'src/app/_models/User';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
  user: User;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  
  constructor(private userService: UserService,
    private alertify: AlertifyService,
    private activatedRouter: ActivatedRoute,
    private route: Router
    ) { }

  ngOnInit() {
    this.loadUser();

    

    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        thumbnailsColumns: 4,
        imagePercent: 100,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false
      }
    ];

    this.galleryImages = this.getImages();
  }

  loadUser(){
    //let id = this.activatedRouter.snapshot.params['id']
    //this.userService.getUser(id).subscribe(resp => this.user = resp);
    this.user = this.activatedRouter.snapshot.data['userResolver'];
    
  }

  getImages()
  {
    const imagesUrl = [];
    for(const photo of this.user.photos){
      imagesUrl.push({
        small: photo.url,
        medium: photo.url,
        big: photo.url,
        description: photo.description
      });
    }
    return imagesUrl;
  }

}
