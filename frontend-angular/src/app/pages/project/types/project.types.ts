export interface ProjectCardDto {
    id: number;
    title: string;
    description: string | null;
    imageUrl: string | null;
    repositoryUrl: string | null;
    type: string;
    stacks: StackDto[];
}

export interface StackDto {
    id: number;
    summary: string;
    category: string;
    skills: SkillRefDto[];
}

export interface SkillRefDto {
    name: string;
    imageUrl: string | null;
}