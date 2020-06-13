import { Component, OnInit, ViewChild } from '@angular/core';
import { NgxGalleryAnimation, NgxGalleryOptions, NgxGalleryImage } from 'ngx-gallery';
import { ActivatedRoute, Router, ActivatedRouteSnapshot } from '@angular/router';
import { Route } from '@angular/compiler/src/core';

import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { User } from 'src/app/_models/User';
import { TabsetComponent } from 'ngx-bootstrap/tabs';


@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
  @ViewChild('memberTabs') memberTabs: TabsetComponent;
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

    const tab = this.activatedRouter.snapshot.queryParams['tab'];
    this.memberTabs.tabs[tab > 0 ? tab : 0].active = true;

    

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

  selectTab(tabId: number){
    this.memberTabs.tabs[tabId].active = true;
  }

}
