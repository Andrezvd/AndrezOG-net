export interface AdminStackDto {
  id: number;
  summary: string;
  category: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
  skills: AdminSkillRefDto[];
}

export interface AdminSkillRefDto {
  id: number;
  name: string;
  imageUrl: string | null;
}

export interface CreateStackPayload {
  summary: string;
  category: string;
  isActive: boolean;
  skillIds: number[];
}