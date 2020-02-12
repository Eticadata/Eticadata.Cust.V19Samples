# ![](.\eticadata.png) eticadata ERP

## Repositório de exemplos de customização

> A Eticadata disponibiliza para todos os utilizadores do seu ERP um vasto repositório com exemplos de customização. Para os utilizadores que pretendem utilizar os exemplos de customização abaixo apresentados, deverão ter obrigatóriamente o ERP Eticadata instalado.

----------



1.  Exemplo de <span style="color:#075fff">**User Controls**</span>

> O projeto **`Eticadata.Cust.Desktop`** disponibiliza exemplos de como adicionar um novo **User Control** ao ERP. 



Abrir o projeto *Eticadata Customization V18 Samples* no Visual Studio. O projeto depois de compilado criará uma dll "*Eticadata.Cust.DeskTop.dll*" na pasta "*.build*" onde se encontra o projeto aberto (`~\Eticadata Customization V18 Samples\Eticadata.Cust.DeskTop\.build\`). De seguida deverá ser copiada para a pasta bin do ERP `[DRIVER]:\Program Files (x86)\eticadata software\ERP v18\Desktop\bin\`. 

> **NOTA:** No caso de estar em falta alguma referencia no projeto *Eticadata Customization V18 Samples*, deverão ser adicionadas ao projeto as dlls que se encontram na pasta de instalação do ERP Eticadata  `[DRIVER]:\Program Files (x86)\eticadata software\ERP v18\Desktop\bin\`.

Após feita a cópia da dll  "*Eticadata.Cust.DeskTop.dll* para a pasta é necessário abrir o ERP e acrescentar a mesma ao sistema de assemblies do ERP Eticadata (separador Admin \ Customização \ Assemblies).  

   

No ficheiro **`init.cs`**, que está dentro da pasta **`Eticadata.Cust.Desktop`**, poderá encontrar no construtor da class a forma como o contexto aplicacional é disponibilizado na class. 

```csharp  
[InjectionConstructor()]  
public Init([ServiceDependency()] WorkItem myWorkItem_,  
[ServiceDependency()] EtiAplicacao myEtiApp_,  
[ServiceDependency()] UIUtils myUiutils_)  
{  
 myEtiApp = myEtiApp_;  
 myUiutils = myUiutils_;  
 myWorkItem = myWorkItem_;  
}   
```



É também nesta class **`init.cs`** que são referenciadas as janelas customizadas. A função `public void OnShowIntegraDocs` tem um `[CommandHandler("cmdNewWindow")]` que tem a funcionalidade de abrir uma nova janela no ribbon do ERP. 

```csharp  
[CommandHandler("cmdNewWindow")]  
public void OnShowIntegraDocs(object sender, EventArgs e)  
{  
 //Nome do userControl (nova janela a colocar na ribbon)  
 ShowWindow<NewWindow>(false, true);  
}   
```



Para disponibilizar esta janela customizada na ribbon do ERP devemos aceder através do separador Admin \ Editar Ribbon e criar um novo item. Efetuar a configuração do novo item com o comando denomidado de *cmdNewWindow* e gravar as alterações implementadas.  

Depois de terminado as tarefas de customização deveremos fechar o ERP para que este assuma as novas implementações feitas e quando iniciado novamente, será apresentado o novo item customizado na ribbon.



____



2. Exemplo de <span style="color:#075fff">__Webservices__</span>

   > O projeto **`Eticadata.Cust.Webservices`** disponibiliza exemplos de como adicionar um webservice customizado ao ERP.



   Abrir o projeto *Eticadata Customization V18 Samples* no Visual Studio. O projeto depois de compilado criará dlls denominadas "*Eticadata.Cust.WebServices.dll*" na pasta "*.build*" onde se encontra o projeto aberto (`~\Eticadata Customization V18 Samples\Eticadata.Cust.WebServices\.build\`). De seguida deverão ser copiadas para a pasta bin do site do ERP  `[DRIVER]:\eticadata Sites\ERP v17\Eticadata.Web\Bin\`. 

   > **NOTA:** No caso de estar em falta alguma referencia no projeto *Eticadata Customization V18 Samples*, deverão ser adicionadas ao projeto as dlls que se encontram na pasta de instalação do Site ERP Eticadata  `[DRIVER]:\eticadata Sites\ERP v18\Eticadata.Web\Bin\`.

   Adicionalmente é necessário incluir a assembly na lista de assemblies a ser carregadas pelo site. Será necessário editar o ficheiro `Web.Config ` que se encontra na pasta `C:\eticadata Sites\ERP v18\Eticadata.Web`, e adicionar a seguinte linha, conforme o exemplo abaixo:

   ```xml
   <configuration>
       <system.web>
           <compilation>
               <assemblies>
               	<!-- Adicionar a linha abaixo indicada -->
                   <add assembly="Eticadata.Cust.WebServices" />
   ```



   Para ser possível garantirmos que o Webservice apenas esteja disponível após a existência de uma sessão válida no ERP Eticadata, alguns `controllers` podem estar protegidos através da anotação `[Authorize]`.  (Saiba mais em https://docs.microsoft.com/en-us/aspnet/core/security/authorization/roles).

   Sempre que utilizar a anotação `[Authorize]`, os pedidos ao webservice terão que obrigatóriamente passar pela inicialização de uma sessão, através de pedidos a webservices que existem de base no ERP.



   __Autenticação de utilizador__

   O primeiro pedido a ser feito ao webservice é a autenticação de utilizador. É possível efetuar a autenticação através do seguinte url:

   ```xml
   POST    http://localhost/ERPV18/api/Shell/LoginUser/
           
           Content-Type: application/json; charset=UTF-8
   ```

   ~~~json
   //Aplicar bloco de código JSON como parâmetro de acesso. Alterar de acordo com configurações do ERP.
   {   
       "login": "demo",
       "password": "DEMO",
       "idioma": "pt-pt",
       "server": "LOCALHOST\SQLEXPRESS",
       "sistema": "SISTEMA"
   }
   ~~~



   O webservice responde com um JSON onde, entre outras informações, existe um valor boleano que indica se os dados são válidos e se a autenticação foi bem sucedida.



   __Autenticação de base de dados__

   Caso a resposta do webservice anterior seja válida, passamos então para o segundo pedido. Será necessário indicar qual a base de dados sobre a qual serão executadas as operações.  Proceder da mesma forma que é feita no passo anterior, utilizando o url abaixo:

   ```
   POST    http://localhost/ERPV18/api/Shell/OpenCompany/
   
           Content-Type: application/json; charset=UTF-8
   ```

   ~~~json
   //Aplicar bloco de código JSON como parâmetro de acesso. Alterar de acordo com configurações do ERP.
   {
       "reabertura":true,
       "mostrarJanelaIniSessao":false,
       "codEmpresa":"DEMO",
       "codExercicio":"2018",
       "codSeccao":"1"
   }
   ~~~



   Após efetuada a publicação do webservice no site do ERP, é possivel invocar os webservices customizados acedendo ao url com o formato `http://<ServerName>/erpv18/api/<ControllerName>/[actionName]/`.

   Neste caso em concreto, o url do exemplo disponibilizado é `http://localhost/erpv18/api/CustUtilities/PrintReport/`.



   É importante referir que a autenticação é baseada em cookies. Assim sendo, é necessário ter em conta que todos os pedidos aos webservices deve conter no Header os cookies de sessão. É também importante levar em consideração que em cada resposta dos webservices podem ser retornados novos cookies.

   Após a invocações dos webservices necessários, customizados ou não, é imprescinvidel terminar a sessão. Esta operação é feita através da invocação de um webservice de Logout:

   ```
   POST    http://localhost/ERPV18/api/Shell/LogoutUser/
   
           Content-Type: application/json; charset=UTF-8
   ```



   Este webservice libertará o licenciamento utilizado durante as operações realizadas no ambito da sessão, e terminará a sessão.



____

