import { User } from './User';
import { firestore } from 'firebase';
import { Location } from './Location';
import { Playlist } from './Playlist';


export interface IPlace {
  displayName?: string | undefined;
  address?: string | undefined;
  phone?: string | undefined;
  id?: string | undefined;
  rating?: number | undefined;
  location?: Location | undefined;
  geohash?: string | undefined;
  user?: User | undefined;
}


export class Place implements IPlace {
  public displayName?: string;
  public address?: string;
  public phone?: string;
  public id: string;
  public rating?: number;
  public location?: Location = new Location();
  public playlist?: Playlist = new Playlist();
  public geohash?: string;
  public user?: User;

  constructor(data?: IPlace) {
    if (data) {
      for (const property in data) {
        if (data.hasOwnProperty(property)) {
          (<any>this)[property] = (<any>data)[property];
        }
      }
    }
  }

  toJson(data?: any) {
    data = typeof data === 'object' ? data : {};
    data['displayName'] = this.displayName;
    data['address'] = this.address;
    data['phone'] = this.phone;
    data['rating'] = this.rating;
    data['location'] = this.location ? new firestore.GeoPoint(this.location.latitude, this.location.longitude) : undefined;
    data['geohash'] = this.geohash;
    data['user'] = this.user ? this.user.toJson(data) : undefined;
    return data;
  }

}
