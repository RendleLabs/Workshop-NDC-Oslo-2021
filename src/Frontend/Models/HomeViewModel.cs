namespace Frontend.Models;

public class HomeViewModel
{
    public HomeViewModel()
    {
            
    }
    public HomeViewModel(List<ToppingViewModel> toppings, List<CrustViewModel> crusts)
    {
        Toppings = toppings;
        Crusts = crusts;
    }

    public List<ToppingViewModel> Toppings { get; set; }
    public List<CrustViewModel> Crusts { get; set; }

    public string SelectedCrust { get; set; }
}