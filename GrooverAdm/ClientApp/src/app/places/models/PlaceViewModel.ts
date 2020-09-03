import { Place, IPlace } from 'src/app/services/services';


export class PlaceViewModel extends Place {
  showEdit = false;

  constructor(data?: IPlace) {
    super(data);
    if (!!this.mainPlaylist) {
      this.mainPlaylist.id = 'spotify:playlist:' + this.mainPlaylist.id;
    }
  }

  checkUserOwner(id: string): void {
    this.showEdit = this.owners.some(o => o.id === id);
  }
}
