using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;

namespace Flotomachine.Elements;

public class FillingWrapPanel : WrapPanel
{
	// Этап подсчета занимаемого места
	protected override Size MeasureOverride(Size availableSize)
	{
		double width = 0, maxWidth = 0, y = 0, nextY = 0;
		foreach (IControl child in Children)
		{
			// Запрашиваем измерение желаемого размера
			child.Measure(availableSize);
			// Если элемент помещается в текущую строку
			if (width + child.DesiredSize.Width <= availableSize.Width)
				// увеличиваем ширину текущей строки
				width += child.DesiredSize.Width;
			// иначе
			else
			{
				// предполагаем размещение элемента в следующей строке
				width = child.DesiredSize.Width;
				y = nextY;
			}
			// Запоминаем максимальную потребную ширину
			if (width > maxWidth)
				maxWidth = width;
			// и высоту панели
			if (y + child.DesiredSize.Height > nextY)
				nextY = y + child.DesiredSize.Height;
		}
		return new Size(maxWidth, nextY);
	}

	// Этап размещения элементов
	protected override Size ArrangeOverride(Size finalSize)
	{
		double y = 0, nextY = 0, width = 0;
		// Коллекция, содержащая элементы текущей строки
		var line = new List<IControl>();
		foreach (IControl child in Children)
		{
			// Если элемент уместится в текущей строке
			if (width + child.DesiredSize.Width <= finalSize.Width)
			{
				// Помещаем его в список элементов
				line.Add(child);
				// Подсчитываем потребную ширину строки
				width += child.DesiredSize.Width;
				// Подсчитываем смещение по вертикали следущей строки
				if (y + child.DesiredSize.Height > nextY)
					nextY = y + child.DesiredSize.Height;
			}
			// иначе
			else
			{
				// Размещаем строку
				ArrangeLine(line, finalSize.Width, y, nextY - y);
				// Элементы следующей строки
				line = new List<IControl> { child };
				width = child.DesiredSize.Width;
				y = nextY;
				nextY = y + child.DesiredSize.Height;
			}
		}
		// Последняя строка
		ArrangeLine(line, finalSize.Width, y, nextY - y);
		return finalSize;
	}

	private void ArrangeLine(List<IControl> line, double lineWidth, double y, double lineHeight)
	{
		// Потребная ширина строки
		double width = line.Sum(fe => fe.DesiredSize.Width);
		// Делим оставшуюся часть длины строки на все элементы
		double delta = (lineWidth - width) / line.Count;
		// Смещение элемента внутри строки
		double x = 0;
		foreach (var fe in line)
		{
			// Вычисляем ширину текущего элемента
			double curWidth = fe.DesiredSize.Width + delta;
			// Размещаем элемент
			fe.Arrange(new Rect(x, y, curWidth, lineHeight));
			// Вычисляем смещение следующего
			x += curWidth;
		}
	}
}