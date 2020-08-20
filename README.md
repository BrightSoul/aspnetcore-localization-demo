# ASP.NET Core Localization Demo
Un'applicazione dimostrativa ASP.NET Core 3.1 che localizza sia i contenuti che i percorsi in tre lingue.

![demo.gif](demo.gif)

I contenuti si trovano in **file di risorse** all'interno della directory `Resources`. Leggi questo articolo propedeutico che spiega come creare e modificare i file di risorse.

https://www.aspitalia.com/script/1333/Usare-File-Risorse-ASP.NET-Core.aspx

All'interno di ciascun file di risorse si trovano sia le chiavi per i contenuti e sia i percorsi. Ecco un estratto di codice da [Resources/Shared.it.resx](Resources/Shared.it.resx):

```
  <data name="Chapter2.Title" xml:space="preserve">
    <value>Capitolo 2</value>
  </data>
  <data name="Chapter2.Text" xml:space="preserve">
    <value>Così ho trascorso la mia vita...</value>
  </data>
  <data name="Routing.Book" xml:space="preserve">
    <value>Libro</value>
  </data>
  <data name="Routing.Book.Chapter1" xml:space="preserve">
    <value>Capitolo1</value>
  </data>
```

Come si vede, le chiavi `Routing.Book` e `Routing.Book.Chapter1` identificano rispettivamente i nomi che il controller `BookController` e l'action `Chapter1` assumeranno nella barra degli indirizzi del browser.


## Avviare l'applicazione
È sufficiente clonare il repository o scaricare il pacchetto zip. Poi aprire il progetto con Visual Studio o Visual Studio Code e premere `F5` per avviare il debug.

## Punti salienti

### Localizzazione dei contenuti testuali

### Impostazione della Culture appropriata

### Configurazione della route MVC

### Riscrittura degli URL localizati

### Riscrittura dei link


## Todo
Questa demo *NON* copre:
 * Aree;
 * Razor Pages;
 * Navigazioni originate da form;
 * Controller presenti in altri progetti correlati;

> **Il codice dell'applicazione non è coperto da test automatici perciò non è pronto per essere usato in produzione**.


## Link utili
 * Articolo su Dynamic Controller Routing (la tecnica usata in questa applicazione)
   
   https://www.strathweb.com/2019/08/dynamic-controller-routing-in-asp-net-core-3-0/

 * Un'altra applicazione simile ma per ASP.NET Core 2.x.

   https://github.com/saaratrix/asp.net-core-mvc-localized-routing

 * Issue su Dynamic Controller Routing che obbliga ad aggiungere anche una route tradizionale oltre alla route dinamica, pena l'impossibilità di generare URL da `AnchorTagHelper` o `LinkGenerator`.
  
   https://github.com/dotnet/aspnetcore/issues/16965
   
 * Issue a proprosito dell'obsolescenza del metodo `WithCulture` dell'`IStringLocalizer`, usato in più punti in questa applicazione.
   
   https://github.com/dotnet/aspnetcore/issues/7756
