export interface ChoreDto {
  id: string;
  title: string | null;
  description: string | null;
  dueDate: string | null;
  conclusionDate: string | null;
}

export interface CreateChoreInput {
  title: string;
  description: string;
  dueDate?: string | null;
}

export interface CreateChoreOutput {
  id: string;
}

export interface ListChoresQuery {
  Title?: string;
  Done?: boolean;
}
