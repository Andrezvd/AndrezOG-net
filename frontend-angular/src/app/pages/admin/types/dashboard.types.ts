export interface SkillDto {
  id: number;
  name: string;
  skillType: string;
  description: string | null;
  isActive: boolean;
  imageUrl: string | null;
}