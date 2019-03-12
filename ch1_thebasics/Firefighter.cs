public class Firefighter : PublicServant, IPerson
{
    public Firefighter(string name, int age)
    {
        this.Name = name;
        this.Age = age;
    }

    public string Name { get; set; }
    public int Age { get; set; }

    public override void DriveToPlaceOfInterest()
    {
        GetInFiretruck();
        TurnOnSiren();
        FollowDirections();
    }

    private void GetInFiretruck() { }
    private void TurnOnSiren() { }
    private void FollowDirections() { }
}

