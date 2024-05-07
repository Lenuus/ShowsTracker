import { Category } from "../enums/category-enum";
import { ReleaseType } from "../enums/release-type-enum";
import { Status } from "../enums/status-enum";
import { TrackStatus } from "../enums/track-status-enum";
import { ShowLinkModel } from "../show/show-link-model";
import MyShowListGenreModel from "./my-show-list-response-genre-model";

class MyShowListModel {
    public id: string = "";
    public name: string = "";
    public totalEpisode: number = 0;
    public currentEpisode: number = 0;
    public category: Category = 0;
    public status: Status = 0;
    public coverImageUrl: string = "";
    public rating: number = 0;
    public releaseGap: number = 0;
    public releaseType: ReleaseType = 0;
    public links: ShowLinkModel[] = [];
    public lastUpdateDate!: Date;
    public trackStatus!: TrackStatus;
    public showId!: string;
    public genres: MyShowListGenreModel[] = [];
}

export { MyShowListModel }