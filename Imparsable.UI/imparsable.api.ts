import { GeneratorConfig } from 'ng-openapi';

const config: GeneratorConfig = {
  input: '../Imparsable.API/Imparsable.API.json',
  output: './src/api/imparsable',
  options: {
    dateType: 'Date',
    enumStyle: 'enum',
    generateEnumBasedOnDescription: true
  }
};

export default config;
