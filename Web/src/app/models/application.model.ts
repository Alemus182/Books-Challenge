export class Application {
    idApplication: number;
    name: string;
    description: string;
    code: string;
    constructor(
      idApplication: number,
      name: string,
      description: string,
      code: string
    ){
      this.idApplication = idApplication;
      this.name = name;
      this.code = code;
      this.description = description;
    }
  }
  