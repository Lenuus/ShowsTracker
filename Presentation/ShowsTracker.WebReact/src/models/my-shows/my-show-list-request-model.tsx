import { Category } from "../enums/category-enum";
import { ReleaseType } from "../enums/release-type-enum";
import { Status } from "../enums/status-enum";
import { TrackStatus } from "../enums/track-status-enum";
import { PagedRequestModel } from "../paged-request-model";

export class MyShowListRequestModel extends PagedRequestModel {
    public search: string = "";
    public genres: string[] = [];
    public statuses: Status[] = [];
    public categories: Category[] = [];
    public startRating: number | undefined;
    public endRating: number | undefined;
    public trackStatuses: TrackStatus[] = [];
}