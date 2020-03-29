

export class User {
  public name: string;
  public userName: string;
  public id: string;

  toJson(data: any): any {
    data = typeof data === 'object' ? data : {};
    data['name'] = this.name;
    data['userName'] = this.userName;
    data['id'] = this.id;
    return data;
  }
}
