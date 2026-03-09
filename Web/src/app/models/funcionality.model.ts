export class Funcionality {
    idFuncionality: number;
    idApplication: number;
    name: string;
    code: string;
    description: string;
    created: Date;
    createdBy: string;
    updated: Date;
    updatedBy: string;
    active: boolean;
    constructor(
      idFuncionality: number,
      idApplication: number,     
      name: string,
      code: string,
      description: string,
      created: Date,
      createdBy: string,
      updated: Date,
      updatedBy: string,
      active: boolean
    ){
      this.idFuncionality = idFuncionality;
      this.idApplication = idApplication;
      this.name = name;
      this.code = code;
      this.description = description;
      this.created = created;
      this.createdBy = createdBy;
      this.updated = updated;
      this.updatedBy = updatedBy;
      this.active = active;
    }
}
  