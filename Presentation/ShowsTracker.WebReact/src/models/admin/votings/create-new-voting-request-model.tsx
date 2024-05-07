export default class CreateNewVotingRequestModel {
    public name!: string;
    public startDate!: Date;
    public endDate!: Date;
    public shows: CreateNewVotingRequestShowModel[] = [];
}

export class CreateNewVotingRequestShowModel {
    public id!: string;
    public name!: string;
    public displayOrder!: number;
}