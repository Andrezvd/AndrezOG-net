export interface AdminProjectDto {
  id: number;
  title: string;
  description: string | null;
  startDate: string;
  endDate: string;
  isActive: boolean;
  repositoryUrl: string | null;
  type: string;
  imageUrl: string | null;
  stacks: AdminStackDto[];
}

export interface AdminStackDto {
  id: number;
  summary: string;
  category: string;
  skills: AdminSkillRefDto[];
}

export interface AdminSkillRefDto {
  id: number;
  name: string;
  imageUrl: string | null;
}

export interface StackOptionDto {
  id: number;
  summary: string;
}

export interface CreateProjectPayload {
  title: string;
  description: string | null;
  startDate: string;
  endDate: string;
  isActive: boolean;
  repositoryUrl: string | null;
  type: string;
  imageFile: File | null;
  stackIds: number[];
}