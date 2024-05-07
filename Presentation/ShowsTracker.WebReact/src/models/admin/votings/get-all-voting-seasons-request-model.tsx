import { PagedRequestModel } from "../../paged-request-model";

export default class GetAllVotingSeasonsRequestModel extends PagedRequestModel {
    public startDate?: Date;
    public endDate?: Date;
    public name?: string;
    public isFinished?: boolean;
}