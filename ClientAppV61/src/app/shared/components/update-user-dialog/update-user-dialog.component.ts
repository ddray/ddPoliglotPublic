import { Component, OnInit, Inject } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { IUser } from 'src/api-authorization/authorize.service';
import { UserService } from '../../services/user.service';

interface BoolPare {
  value: string;
  checked: boolean;
}

@Component({
  selector: 'app-update-user-dialog',
  templateUrl: './update-user-dialog.component.html',
  styleUrls: ['./update-user-dialog.component.scss']
})
export class UpdateUserDialogComponent implements OnInit {
  public roles = new Array<BoolPare>();

  constructor(@Inject(MAT_DIALOG_DATA) public data: IUser,
              private dialogRef: MatDialogRef<UpdateUserDialogComponent>,
              private fb: FormBuilder,
              private userService: UserService,
  ) { }

  async ngOnInit() {
    const allRoles = await this.userService.getAllRoles().toPromise();
    // create array with all roles and mark them for user roles
    allRoles.forEach(x => {
      const chk = this.data.roles.some(r => r === x);
      this.roles.push({ value: x, checked: chk} as BoolPare);
    });
  }

  save() {
    console.log('Save user roles');
    this.userService.Save({
        id: this.data.id,
        roles: this.roles.filter(x => x.checked).map(x => x.value),
      } as IUser
    ).toPromise()
      .then((x) => {
        this.dialogRef.close(x);
      });
  }

  cancel() {
    this.dialogRef.close(null);
  }
}

