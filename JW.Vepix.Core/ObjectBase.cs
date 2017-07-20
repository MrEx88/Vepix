using JW.Vepix.Core.Extensions;
using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace JW.Vepix.Core
{
    public class ObjectBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "", bool makeDirty = true)
        {
            if (propertyName != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }

            if (makeDirty)
            {
                _isDirty = true;
            }
        }

        protected void NotifyPropertyChanged<T>(Expression<Func<T>> propertyExpression, bool makeDirty = true)
        {
            string propertyName = PropertySupport.ExtractPropertyName(propertyExpression);
            NotifyPropertyChanged(propertyName);
        }

        public bool IsDirty
        {
            get { return _isDirty; }
            set { _isDirty = value; }
        }

        public List<ObjectBase> GetDirtyObjects()
        {
            List<ObjectBase> dirtyObjects = new List<ObjectBase>();

            WalkObjectGraph(o =>
            {
                if (o.IsDirty)
                {
                    dirtyObjects.Add(o);
                }
                return false;
            }, coll => { });

            return dirtyObjects;
        }

        public virtual bool IsAnythingDirty()
        {
            bool isAnythingDirty = false;
            WalkObjectGraph(o =>
            {
                if (o.IsDirty)
                {
                    isAnythingDirty = true;
                    return true;
                }
                else
                {
                    return false;
                }
            }, coll => { });

            return isAnythingDirty;
        }

        public void CleanAll()
        {
            WalkObjectGraph(o =>
            {
                if (o.IsDirty)
                {
                    o.IsDirty = false;
                }

                return false;
            }, col => { });
        }

        protected void WalkObjectGraph(Func<ObjectBase, bool> exitObjectGraph,
            Action<IList> snippetForCollection, params string[] exemptProperties)
        {
            List<ObjectBase> visited = new List<ObjectBase>();
            Action<ObjectBase> walk = null;

            List<string> exemptions = new List<string>();
            exemptions = exemptProperties?.ToList();

            walk = (o) =>
            {
                if (o != null && !visited.Contains(o))
                {
                    visited.Add(o);

                    bool exitWalk = exitObjectGraph.Invoke(o);
                    if (!exitWalk)
                    {
                        PropertyInfo[] properties = o.GetBrowsableProperties();
                        foreach (PropertyInfo property in properties)
                        {
                            if (!exemptions.Contains(property.Name))
                            {
                                if (property.PropertyType.IsSubclassOf(typeof(ObjectBase)))
                                {
                                    ObjectBase obj = (ObjectBase)property.GetValue(o, null);
                                    walk(obj);
                                }
                                else
                                {
                                    IList coll = property.GetValue(o, null) as IList;
                                    if (coll != null)
                                    {
                                        snippetForCollection.Invoke(coll);

                                        foreach (object item in coll)
                                        {
                                            if (item is ObjectBase)
                                            {
                                                walk((ObjectBase)item);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            walk(this);
        }

        private bool _isDirty;
    }
}
