import { Category } from "../enums/category-enum";
import { ReleaseType } from "../enums/release-type-enum";
import { Status } from "../enums/status-enum";
import { ShowLinkModel } from "./show-link-model";
import ShowListGenreModel from "./show-list-genre-model";

class ShowListModel {
    public id: string = "";
    public name: string = "";
    public totalEpisode: number = 0;
    public currentEpisode: number | undefined;
    public category: Category = 0;
    public status: Status = 0;
    public coverImageUrl: string = "";
    public rating: number = 0;
    public releaseGap: number = 0;
    public releaseType: ReleaseType = 0;
    public links: ShowLinkModel[] = [];
    public isFollowedByCurrentUser: boolean = false;
    public genres: ShowListGenreModel[] = [];
}

export { ShowListModel }