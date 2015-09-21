using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Ark.TemplateSelectors
{
    public class noteActionTemplateSelector : DataTemplateSelector
    {
        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            //ImageFavorite obj = item as ImageFavorite;
            ContentPresenter contentPresenter = (ContentPresenter)container;
            DataTemplate dataTemplate = new DataTemplate();

            NoteAction noteAction = (NoteAction)item;

            if (noteAction.ActionType == NoteAction.ActionTypeEnum.CheckboxFalse)
            {
                dataTemplate = contentPresenter.FindResource("dt_noteAction_checkboxItemFalse") as DataTemplate;
            }
            else if (noteAction.ActionType == NoteAction.ActionTypeEnum.CheckboxTrue)
            {
                dataTemplate = contentPresenter.FindResource("dt_noteAction_checkboxItemTrue") as DataTemplate;
            }
            else
            {
                dataTemplate = contentPresenter.FindResource("dt_noteAction_item") as DataTemplate;
            }

            return dataTemplate;
        }

    }
}
