import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Store } from '@ngrx/store';
import { loadCurrentUser } from '../store/actions/user.actions';

@Component({
  imports: [RouterOutlet],
  selector: 'app-root',
  styleUrl: './app.scss',
  templateUrl: './app.html',
})
export class App implements OnInit {
  private store = inject(Store);

  protected readonly title = signal('identity-client');

  ngOnInit(): void {
    this.store.dispatch(loadCurrentUser());
  }
}
