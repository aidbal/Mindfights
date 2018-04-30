import { Component, Injector, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { MenuItem } from '@shared/layout/menu-item';

@Component({
    templateUrl: './sidebar-nav.component.html',
    selector: 'sidebar-nav',
    encapsulation: ViewEncapsulation.None
})
export class SideBarNavComponent extends AppComponentBase {

    menuItems: MenuItem[] = [
        new MenuItem(this.l("Protmūšiai"), "", "videogame_asset", "/app/mindfights"),
        new MenuItem(this.l("Būsimi komandos protmūšiai"), "", "grade", "/app/mindfights/registered"),
        new MenuItem(this.l("Protmūšiai (vertinimas)"), "CreateMindfights", "check", "/app/mindfights/evaluate"),
        new MenuItem(this.l("Protmūšiai (valdymas)"), "CreateMindfights", "edit", "/app/mindfights/administrate"),
        new MenuItem(this.l("Komanda"), "", "group", "/app/team"),
        new MenuItem(this.l("Žaidėjas"), "", "face", "/app/player"),
        new MenuItem(this.l("Vartotojai"), "Pages.Users", "people", "/app/users"),
        new MenuItem(this.l("Rolės"), "Pages.Roles", "local_offer", "/app/roles")
    ];

    constructor(
        injector: Injector
    ) {
        super(injector);
    }

    showMenuItem(menuItem): boolean {
        if (menuItem.permissionName) {
            return this.permission.isGranted(menuItem.permissionName);
        }

        return true;
    }
}
