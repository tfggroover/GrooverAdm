import { User } from './User';
import { Location } from './Location';


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
  public location?: Location;
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

}
