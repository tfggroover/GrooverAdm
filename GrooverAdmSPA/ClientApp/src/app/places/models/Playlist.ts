
export class Playlist {
  public name?: string;
  public url?: string;
  public imageUrl?: string;
  public checked?: boolean;

  toJson(data?: any) {
    data = typeof data === 'object' ? data : {};
    data['name'] = this.name;
    data['url'] = this.url;
    data['imageURl'] = this.imageUrl;
    data['checked'] = this.checked;
    return data;
  }
}
