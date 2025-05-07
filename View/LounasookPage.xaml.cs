﻿using System.Globalization;
using MauiApp4.Database;
using MauiApp4.Models;

namespace MauiApp4.View;

public partial class LounasookPage : ContentPage
{
    private string lisafoto;
    private byte[] fotoBytes;
    private LounasookDatabase database;
    private LounasookClass selectedItem;

    private EntryCell ec_roaNimi, ec_valgud, ec_rasvad, ec_susivesikud, ec_kalorid;
    private DatePicker dp_kuupaev;
    private TimePicker tp_kallaaeg;
    private Image img;

    private TableView tableview;
    private TableSection fotoSection;
    private ListView lounasookListView;

    private Button btn_salvesta, btn_kustuta, btn_puhastada, btn_pildista, btn_valifoto, btn_hide;

    public LounasookPage()
    {
        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Tervisepaevik.db");
        database = new LounasookDatabase(dbPath);

        Title = "Lounasook";

        ec_roaNimi = new EntryCell { Label = "Roa nimi", Placeholder = "nt. Puder" };
        ec_valgud = new EntryCell { Label = "Valgud", Placeholder = "g", Keyboard = Keyboard.Numeric };
        ec_rasvad = new EntryCell { Label = "Rasvad", Placeholder = "g", Keyboard = Keyboard.Numeric };
        ec_susivesikud = new EntryCell { Label = "Süsivesikud", Placeholder = "g", Keyboard = Keyboard.Numeric };
        ec_kalorid = new EntryCell { Label = "Kalorid", Placeholder = "kcal", Keyboard = Keyboard.Numeric };

        dp_kuupaev = new DatePicker { Date = DateTime.Now };
        tp_kallaaeg = new TimePicker { Time = TimeSpan.FromHours(8) };

        btn_salvesta = new Button { Text = "Salvesta" };
        btn_kustuta = new Button { Text = "Kustuta", IsVisible = false };
        btn_puhastada = new Button { Text = "Uus sisestus" };
        btn_pildista = new Button { Text = "Tee foto" };
        btn_valifoto = new Button { Text = "Vali foto" };
        btn_hide = new Button { Text = "Näita loendit" };

        btn_salvesta.Clicked += Btn_salvesta_Clicked;
        btn_kustuta.Clicked += Btn_kustuta_Clicked;
        btn_puhastada.Clicked += Btn_puhastada_Clicked;
        btn_pildista.Clicked += Btn_pildista_Clicked;
        btn_valifoto.Clicked += Btn_valifoto_Clicked;
        btn_hide.Clicked += Btn_hide_Clicked;

        img = new Image();

        fotoSection = new TableSection("Foto");

        tableview = new TableView
        {
            Intent = TableIntent.Form,
            Root = new TableRoot("Sisesta Lounasook")
            {
                new TableSection("Üldandmed")
                {
                    new ViewCell { View = dp_kuupaev },
                    new ViewCell { View = tp_kallaaeg },
                    ec_roaNimi,
                    ec_valgud,
                    ec_rasvad,
                    ec_susivesikud,
                    ec_kalorid
                },
                fotoSection,
                new TableSection("Tegevused")
                {
                    new ViewCell
                    {
                        View = new StackLayout
                        {
                            Orientation = StackOrientation.Horizontal,
                            HorizontalOptions = LayoutOptions.Center,
                            Children = { btn_salvesta, btn_kustuta, btn_puhastada }
                        }
                    }
                },
                new TableSection("FOTO")
                {
                    new ViewCell
                    {
                        View = new StackLayout
                        {
                            Orientation = StackOrientation.Horizontal,
                            HorizontalOptions = LayoutOptions.Center,
                            Children = { btn_valifoto, btn_pildista }
                        }
                    }
                }
            }
        };

        lounasookListView = new ListView
        {
            SeparatorColor = Colors.DarkViolet,
            BackgroundColor = Colors.WhiteSmoke,
            Header = "Lounasöögid",
            HasUnevenRows = true,
            ItemTemplate = new DataTemplate(() =>
            {
                // Картинка
                Image img = new Image { WidthRequest = 60, HeightRequest = 60 };
                img.SetBinding(Image.SourceProperty, new Binding("Toidu_foto", converter: new ByteArrayToImageSourceConverter()));

                // Название блюда
                Label nimi = new Label { FontSize = 16, FontAttributes = FontAttributes.Bold };
                nimi.SetBinding(Label.TextProperty, "Roa_nimi");

                // Калории
                Label kalorid = new Label { FontSize = 14 };
                kalorid.SetBinding(Label.TextProperty, new Binding("Kalorid", stringFormat: "Kalorid: {0}"));

                // Дата
                Label kuupaev = new Label { FontSize = 14 };
                kuupaev.SetBinding(Label.TextProperty, new Binding("Kuupaev", stringFormat: "Kuupäev: {0:d}"));

                return new ViewCell
                {
                    View = new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        Padding = new Thickness(10),
                        Children =
                {
                    img,
                    new StackLayout
                    {
                        Orientation = StackOrientation.Vertical,
                        Padding = new Thickness(10, 0),
                        Children = { nimi, kalorid, kuupaev }
                    }
                }
                    }
                };
            })
        };



        lounasookListView.ItemSelected += LounasookListView_ItemSelected;

        Content = new ScrollView
        {
            Content = new StackLayout
            {
                Padding = 10,
                Children =
                    {
                        btn_hide,
                        tableview,
                        new Label { Text = "Salvestatud lõunasöögid", FontAttributes = FontAttributes.Bold },
                        lounasookListView
                    }
            }
        };

        AndmeteLaadimine();
    }


    private void Btn_puhastada_Clicked(object sender, EventArgs e) => SelgeVorm();

    private async void Btn_valifoto_Clicked(object sender, EventArgs e)
    {
        FileResult foto = await MediaPicker.Default.PickPhotoAsync();
        await SalvestaFoto(foto);
    }

    private async void Btn_pildista_Clicked(object sender, EventArgs e)
    {
        if (MediaPicker.Default.IsCaptureSupported)
        {
            FileResult foto = await MediaPicker.Default.CapturePhotoAsync();
            await SalvestaFoto(foto);
        }
        else
        {
            await Shell.Current.DisplayAlert("Viga", "Teie seade ei ole toetatud", "Ok");
        }
    }

    private async Task SalvestaFoto(FileResult foto)
    {
        if (foto != null)
        {
            lisafoto = Path.Combine(FileSystem.CacheDirectory, foto.FileName);

            using Stream sourceStream = await foto.OpenReadAsync();
            using MemoryStream ms = new MemoryStream();
            await sourceStream.CopyToAsync(ms);
            fotoBytes = ms.ToArray();

            File.WriteAllBytes(lisafoto, fotoBytes);

            img.Source = ImageSource.FromFile(lisafoto);  // Обновляем источник изображения

            fotoSection.Clear();
            var imageViewCell = new ViewCell
            {
                View = img  // Добавляем Image вместо ImageCell
            };
            fotoSection.Add(imageViewCell);  // Добавляем ViewCell с Image

            await Shell.Current.DisplayAlert("Edu", "Foto on edukalt salvestatud", "OK");
        }
    }

    private void Btn_salvesta_Clicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(ec_roaNimi.Text)) return;

        if (selectedItem == null)
            selectedItem = new LounasookClass();

        selectedItem.Roa_nimi = ec_roaNimi.Text;
        selectedItem.Valgud = int.TryParse(ec_valgud.Text, out var valgud) ? valgud : 0;
        selectedItem.Rasvad = int.TryParse(ec_rasvad.Text, out var rasvad) ? rasvad : 0;
        selectedItem.Susivesikud = int.TryParse(ec_susivesikud.Text, out var susivesikud) ? susivesikud : 0;
        selectedItem.Kalorid = int.TryParse(ec_kalorid.Text, out var kalorid) ? kalorid : 0;
        selectedItem.Kuupaev = dp_kuupaev.Date;
        selectedItem.Kallaaeg = tp_kallaaeg.Time;

        if (fotoBytes != null)
            selectedItem.Toidu_foto = fotoBytes;

        database.SaveLounasook(selectedItem);
        SelgeVorm();
        AndmeteLaadimine();
    }

    private void Btn_kustuta_Clicked(object? sender, EventArgs e)
    {
        if (selectedItem != null)
        {
            database.DeleteLounasook(selectedItem.Lounasook_id);
            SelgeVorm();
            AndmeteLaadimine();
        }
    }

    private void ClearButton_Clicked(object? sender, EventArgs e)
    {
        SelgeVorm();
    }

    private int imageCounter = 1; // Счётчик для имен файлов

    private void LounasookListView_ItemSelected(object? sender, SelectedItemChangedEventArgs e)
    {
        selectedItem = e.SelectedItem as LounasookClass;
        if (selectedItem != null)
        {
            ec_roaNimi.Text = selectedItem.Roa_nimi;
            ec_valgud.Text = selectedItem.Valgud.ToString();
            ec_rasvad.Text = selectedItem.Rasvad.ToString();
            ec_susivesikud.Text = selectedItem.Susivesikud.ToString();
            ec_kalorid.Text = selectedItem.Kalorid.ToString();
            dp_kuupaev.Date = selectedItem.Kuupaev;
            tp_kallaaeg.Time = selectedItem.Kallaaeg;
            btn_kustuta.IsVisible = true;

            if (selectedItem.Toidu_foto != null && selectedItem.Toidu_foto.Length > 0)
            {
                fotoSection.Clear();

                // Генерация уникального имени файла (можно привязать к ID или дате)
                string imageFileName = $"img_{Guid.NewGuid()}.jpg";
                string imagePath = Path.Combine(FileSystem.AppDataDirectory, imageFileName);

                // Сохраняем в постоянную директорию
                File.WriteAllBytes(imagePath, selectedItem.Toidu_foto);

                // Создаём Image и обновляем источник
                var newImage = new Image
                {
                    Source = ImageSource.FromFile(imagePath),
                    HeightRequest = 60,
                    WidthRequest = 60,
                    Aspect = Aspect.AspectFill
                };

                var imageViewCell = new ViewCell { View = newImage };
                fotoSection.Add(imageViewCell);
            }
            else
            {
                fotoSection.Clear();
            }
        }
    }


    public void AndmeteLaadimine()
    {
        lounasookListView.ItemsSource = database.GetLounasook().OrderByDescending(x => x.Kuupaev).ToList();
    }

    public void SelgeVorm()
    {
        selectedItem = null;
        fotoBytes = null;
        ec_roaNimi.Text = ec_valgud.Text = ec_rasvad.Text = ec_susivesikud.Text = ec_kalorid.Text = string.Empty;
        dp_kuupaev.Date = DateTime.Now;
        tp_kallaaeg.Time = TimeSpan.FromHours(8);
        lounasookListView.SelectedItem = null;
        btn_kustuta.IsVisible = false;
        fotoSection.Clear();
    }
    private void Btn_hide_Clicked(object sender, EventArgs e)
    {
        // Переключение видимости для обоих элементов
        tableview.IsVisible = !tableview.IsVisible;
        lounasookListView.IsVisible = !lounasookListView.IsVisible;

        // Изменение текста кнопки в зависимости от текущего состояния
        if (tableview.IsVisible)
        {
            btn_hide.Text = "Näita loendit"; // если tableview виден
        }
        else
        {
            btn_hide.Text = "Näita sisestust"; // если tableview скрыт
        }
    }

    public class ByteArrayToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is byte[] bytes && bytes.Length > 0)
                return ImageSource.FromStream(() => new MemoryStream(bytes));
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
