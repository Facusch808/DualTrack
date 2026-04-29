using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Linq;
using Avalonia.Platform.Storage;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Media;
namespace DualTrack;


public partial class MainWindow : Window
{   
    string RutaFinal= "";
    bool Mp4Mkv=false;
    string idioma= "";
    string RutaLat="";
    string RutaEn="";

    

    public MainWindow()
    {
        InitializeComponent();
    }

       private void Vd_a_ext_audio_lat(object? sender, RoutedEventArgs e) //boton lat
    {
        idioma="lat";
        SelecionDeArchivo();
    }
        private void Vd_a_ins_audio_ing(object? sender, RoutedEventArgs e)//boton ingles
    {
        idioma="en";
        SelecionDeArchivo();
    }

    public async void SelecionCarpeta(object? sender, RoutedEventArgs e)
    {
        var VentActual = TopLevel.GetTopLevel(this); // establece la ventana en la que

        if (VentActual == null) // por si es nulo q no rompa todo
        {
            return;
        }

        var carpeta = await VentActual.StorageProvider.OpenFolderPickerAsync(
            new FolderPickerOpenOptions
            {
                 Title = "Seleccionar carpeta",
                AllowMultiple = false
            });
            var semiRuta = carpeta.FirstOrDefault(); //agrarra de carpeta, osea la seleccion q hiciste y toma la primera.
        if (semiRuta != null) // comprueba si seleccionaste algo, xq sino rompe todo
        {
            RutaFinal = semiRuta.Path.LocalPath; //agarra esa carpeta q tenes y le saca la ruta, de ahi lo convierte a un valor q pueda meter en un string
            RutaFinal = RutaFinal.Substring(0,RutaFinal.Length-1);
            rutadestino.Text = RutaFinal;
        }
    }

    public async void SelecionDeArchivo()
    {
        var VentActual = TopLevel.GetTopLevel(this); // establece la ventana en la que

        if (VentActual == null) // por si es nulo q no rompa todo
        {
            return;
        }

        var Archivo = await VentActual.StorageProvider.OpenFilePickerAsync(
            new FilePickerOpenOptions
            {
                Title="Selecionar video",
                AllowMultiple= false
            });// seleciona el archivo

        if (Archivo.Count > 0)//si se seleciono algo ejecuta esto
        {
            var Lista_de_archivos = Archivo[0];
            if (idioma == "lat")
            {
                RutaLat = Lista_de_archivos.Path.LocalPath;
                latino.Text = RutaLat;
            }
            else
            {
                RutaEn = Lista_de_archivos.Path.LocalPath;
                ingles.Text = RutaEn;
            }
        }
        
    }
    

    public async void Procesar(object? sender, RoutedEventArgs e)
    {
        estado.Text="Iniciando proceso";
        //await Task.Delay(1000);

        if (rutadestino == null)
        {
            return;
        }
        RutaFinal = rutadestino.Text + "/salida.mp4";
        //rutadestino.Text= RutaFinal;
        //RutaFinal= "/home/facu-sch/Escritorio/salida.mp4";
        if(RutaEn!="" && RutaLat != "" && RutaFinal!="")
        {
            
            ComparacionDeVideos();
           // await Task.Delay(1000);
            if(Mp4Mkv==true){
            barraProgreso.IsIndeterminate = true;
            estado.Text="Procesando...";
            await Task.Delay(1000);
            var ffmpeg_ejecucion = new Process();
            ffmpeg_ejecucion.StartInfo.FileName = "ffmpeg";
            ffmpeg_ejecucion.StartInfo.Arguments =
        $"-i \"{RutaEn}\" -i \"{RutaLat}\" " +
        "-map 0:v -map 0:a -map 1:a " +
        "-metadata:s:a:0 language=eng " +
        "-metadata:s:a:0 title=\"English\" " +
        "-metadata:s:a:1 language=spa " +
        "-metadata:s:a:1 title=\"Español Latino\" " +
        $"-c copy \"{RutaFinal}\"";

        ffmpeg_ejecucion.StartInfo.UseShellExecute = false;
        ffmpeg_ejecucion.StartInfo.RedirectStandardError = true;
        ffmpeg_ejecucion.StartInfo.CreateNoWindow = true;
        ffmpeg_ejecucion.Start();
        await ffmpeg_ejecucion.WaitForExitAsync();
        //ffmpeg_ejecucion.WaitForExit();
        //string log = ffmpeg_ejecucion.StandardError.ReadToEnd();
        //log_.Text= log;
        estado.Text="Proceso terminado, su video se a exportado en la ruta destino";
        barraProgreso.IsIndeterminate = false;
        barraProgreso.Value = 100;

            }
            else
            {
                estado.Text="Error, uno de tus archvios no es mp4 o mkv";
            }
        }
        else
        {
            latino.Text="no se a selecionado nada";
            ingles.Text="no se a selecionado nada";
        }
    }

    public async void ComparacionDeVideos()
    {
        if((Path.GetExtension(RutaLat).ToLower() == ".mp4" || Path.GetExtension(RutaLat).ToLower() == ".mkv")  && (Path.GetExtension(RutaEn).ToLower() == ".mp4" || Path.GetExtension(RutaEn).ToLower() == ".mkv"))
        {
            Mp4Mkv=true;

            if (Path.GetExtension(RutaLat).ToLower() == ".mp4" && Path.GetExtension(RutaEn).ToLower() == ".mp4")
             {
            
             }else if(Path.GetExtension(RutaLat).ToLower() == ".mkv" && Path.GetExtension(RutaEn).ToLower() == ".mkv")
            {
            
              }
                else
            {
            if(Path.GetExtension(RutaLat).ToLower() == ".mp4")
            {
                estado.Text="Iniciando conversion a mkv";
                //await Task.Delay(1000);
                string VideoConvertidoMKV = Path.ChangeExtension(RutaLat, ".mkv");
                var Conversion = new Process();
                Conversion.StartInfo.FileName = "ffmpeg";
                Conversion.StartInfo.Arguments = $"-i \"{RutaLat}\" -c copy \"{VideoConvertidoMKV}\"";
                Conversion.StartInfo.UseShellExecute = false;
                Conversion.StartInfo.CreateNoWindow = true;
                Conversion.Start();
                await Conversion.WaitForExitAsync();
                //Conversion.WaitForExit();
                RutaLat=VideoConvertidoMKV;
                
            }
            if(Path.GetExtension(RutaEn).ToLower() == ".mp4")
            {
                estado.Text="Iniciando conversion a mkv";
                //await Task.Delay(1000);
                string VideoConvertidoMKV = Path.ChangeExtension(RutaEn, ".mkv");
                var Conversion = new Process();
                Conversion.StartInfo.FileName = "ffmpeg";
                Conversion.StartInfo.Arguments = $"-i \"{RutaEn}\" -c copy \"{VideoConvertidoMKV}\"";
                Conversion.StartInfo.UseShellExecute = false;
                Conversion.StartInfo.CreateNoWindow = true;
                Conversion.Start();
                await Conversion.WaitForExitAsync();
                //Conversion.WaitForExit();
                RutaEn=VideoConvertidoMKV;
               
            }
         }
        }
        else
        {
            Mp4Mkv=false;
        }
        
    }

}