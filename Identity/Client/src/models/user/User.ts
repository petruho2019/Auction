class User {
  constructor(id: string, email: string, username: string) {
    this.id = id;
    this.email = email;
    this.username = username;
  }
  id: string;
  username: string;
  email: string;
  isAuthenticated: boolean = false;
}
