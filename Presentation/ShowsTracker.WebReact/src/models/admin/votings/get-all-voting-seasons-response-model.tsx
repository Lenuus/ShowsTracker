export default class VoteSeasonDetailDto {
    public id!: string;
    public name!: string;
    public startDate!: Date;
    public endDate!: Date;
    public totalVote!: number;
    public isFinished!: boolean;
    public shows: VoteSeasonDetailShowDto[] = [];
}

export class VoteSeasonDetailShowDto {
    public id!: string;
    public showId!: string;
    public name!: string;
    public isWinner!: boolean;
    public displayOrder!: number;
    public totalVote!: number;
    public coverImageUrl!: string;
    public selectedByCurrentUser: boolean = false;
}