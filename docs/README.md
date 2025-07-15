# Como adicionar documentação ao TechHub

A migração deste repositório conta com uma nova estrutura de documentação, que segue o padrão do [Tech Hub](https://tech-hub.raizen.com/). 

Para adicionar documentação, basta criar um arquivo markdown na pasta `docs` e adicionar o conteúdo no arquivo `mkdocs.yaml` na raiz do repositório. Recomendamos o repositório [plataform-development-pattern](https://github.com/raizen-it/plataform-development-pattern) caso você precise de algum exemplo.

## Arquivos para publicação
Para que a documentação seja publicada no Tech Hub, é necessários que os campos em destaque dos arquivos sejam alterados de acordo com o projeto, no padrão **kebab-case**. 

Por exemplo, se o projeto se chama `svn-Angular-ApplicationName`, ele deve ser alterado para `application-name-interface`. Caso você tenha alguma dúvida quanto ao nome do repositório, consulte a documentação [Padrões de Desenvolvimento de Software Raízen - Nome repositorio](https://tech-hub.raizen.com/docs/default/component/plataform-development-pattern/padroes-dev/nome-repositorio/).

### mkdocs.yaml
Devem ser alterados os campos `site_name` e `site_description` para o nome do projeto.

```diff
+site_name: "${{ projectName }}"
+site_description: "${{ projectName }}"
repo_url: https://github.com/raizen-it/plataform-development-pattern
edit_uri: edit/main/docs

plugins:
  - techdocs-core

nav:
  - Home: README.md
```

### catalog-info.yaml
Devem ser alterados os campos `metadata.name`, `metadata.description` e `metadata.annotations` para o nome do projeto.

```diff
apiVersion: backstage.io/v1alpha1
kind: Component
metadata:
+  name: ${{ projectName }}
+  description: ${{ projectName }}
  annotations:
+    github.com/project-slug: raizen-it/${{ projectName }}
    backstage.io/techdocs-ref: dir:.
  tags:
    - actions
    - security
    - audit
spec:
  type: library
  lifecycle: experimental
  owner: squad-plataforma
  dependsOn:
    [
      "component:action-generate-token-github-app",
      "component:action-content-change-validation",
    ]
```

### .github/workflows/techdocs.yaml
Este arquivo não precisa ser alterado, ele deve ser apenas copiado para o repositório na pasta `.github/workflows/techdocs.yaml`.

```yaml
name: Publish TechDocs Site

on:
  push:
    branches:
      - main
    paths:
      - 'docs/**'
      - 'mkdocs.yaml'

jobs:
  publish-techdocs-site:
    runs-on: ubuntu-latest
    steps:
      - name: Checkout code
        uses: actions/checkout@v3

      - name: Get Kind
        id: catalog-kind
        uses: devorbitus/yq-action-output@v1.1
        with:
          cmd: yq eval '.kind' 'catalog-info.yaml'

      - name: Get Metadata.Name
        id: catalog-metadata-name
        uses: devorbitus/yq-action-output@v1.1
        with:
          cmd: yq eval '.metadata.name' 'catalog-info.yaml'

      - name: Backstage TechDocs
        uses: Staffbase/backstage-techdocs-action@v0.2.0
        with:
          entity-namespace: 'default'
          entity-kind: ${{ steps.catalog-kind.outputs.result }}
          entity-name: ${{ steps.catalog-metadata-name.outputs.result }}
          publisher-type: 'azureBlobStorage'
          storage-name: ${{ secrets.TECHDOCS_CONTAINER_NAME }}
          azure-account-name: ${{ secrets.TECHDOCS_STORAGE_ACCOUNT }}
          azure-account-key: ${{ secrets.TECHDOCS_AZURE_ACCESS_KEY }}
```