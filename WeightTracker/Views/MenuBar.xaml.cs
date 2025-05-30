using CommunityToolkit.Maui.Views;

namespace WeightTracker.Views;

public partial class MenuBar : Popup
{
    static public int type;
	public MenuBar()
	{
		InitializeComponent();
        type = 0;
	}
    private void AddBreakfast_Clicked(object sender, EventArgs e)
    {
        type = 2;
        this.Close();
    }
    private void AddLunch_Clicked(object sender, EventArgs e)
    {
        type = 3;
        this.Close();
    }
    private void AddDinner_Clicked(object sender, EventArgs e)
    {
        type = 4;
        this.Close();
    }
    private void AddSnack_Clicked(object sender, EventArgs e)
    {
        type = 5;
        this.Close();
    }
    private void AddWeight_Clicked(object sender, EventArgs e)
    {
        type = 1;
        this.Close();
    }
    private void AddSport_Clicked(object sender, EventArgs e)
    {
        type = 6;
        this.Close();
        
    }
}