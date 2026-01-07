using UnityEngine;

public interface IFeedable
{
    void Feed();
}

public interface IProduce
{
    bool CanProduce();
    void Produce();   // đã đổi từ GameObject thành void
}

//  Interface mới thêm để Animal nhận sự kiện môi trường
public interface IAnimalEvents
{
    void OnNewDay(int day, int month, int year);
    void OnWeatherChanged(WeatherType newWeather);
    void OnSeasonChanged(Season newSeason);
}

public interface IInteractable
{
    void Interact();
}
