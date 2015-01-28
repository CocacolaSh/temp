using System;
using System.Collections.Generic;
using System.Linq;

namespace Ocean.Core
{
    /// <summary>
    /// Base class for entities
    /// </summary>
    public abstract partial class BaseEntity
    {
        #region 公共属性
        /// <summary>
        /// Gets or sets the entity identifier
        /// 主键Id
        /// </summary>
        public virtual Guid Id 
        {
            get { return GetFieldGuid("Id"); }
            set { SetField("Id", value); }
        }

        private DateTime createDate;
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateDate
        {
            set { createDate = value; }
            get
            {
                if (createDate < new DateTime(1900, 1, 1))
                {
                    createDate = new DateTime(1900, 1, 1);
                }
                return createDate;
            }
        }
        #endregion

        public override bool Equals(object obj)
        {
            return Equals(obj as BaseEntity);
        }

        private static bool IsTransient(BaseEntity obj)
        {
            return obj != null && Equals(obj.Id, default(int));
        }

        private Type GetUnproxiedType()
        {
            return GetType();
        }

        public virtual bool Equals(BaseEntity other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (!IsTransient(this) &&
                !IsTransient(other) &&
                Equals(Id, other.Id))
            {
                var otherType = other.GetUnproxiedType();
                var thisType = GetUnproxiedType();
                return thisType.IsAssignableFrom(otherType) ||
                        otherType.IsAssignableFrom(thisType);
            }

            return false;
        }

        public override int GetHashCode()
        {
            if (Equals(Id, default(int)))
                return base.GetHashCode();
            return Id.GetHashCode();
        }

        public static bool operator ==(BaseEntity x, BaseEntity y)
        {
            return Equals(x, y);
        }

        public static bool operator !=(BaseEntity x, BaseEntity y)
        {
            return !(x == y);
        }

        protected virtual void SetParent(dynamic child)
        {

        }

        protected virtual void SetParentToNull(dynamic child)
        {

        }

        protected void ChildCollectionSetter<T>(ICollection<T> collection, ICollection<T> newCollection) where T : class
        {
            if (CommonHelper.OneToManyCollectionWrapperEnabled)
            {
                collection.Clear();
                if (newCollection != null)
                    newCollection.ToList().ForEach(x => collection.Add(x));
            }
            else
            {
                collection = newCollection;
            }
        }

        protected ICollection<T> ChildCollectionGetter<T>(ref ICollection<T> collection, ref ICollection<T> wrappedCollection) where T : class
        {
            return ChildCollectionGetter(ref collection, ref wrappedCollection, SetParent, SetParentToNull);
        }

        protected ICollection<T> ChildCollectionGetter<T>(ref ICollection<T> collection, ref ICollection<T> wrappedCollection, Action<dynamic> setParent, Action<dynamic> setParentToNull) where T : class
        {
            if (CommonHelper.OneToManyCollectionWrapperEnabled)
                return wrappedCollection ?? (wrappedCollection = (collection ?? (collection = new List<T>())).SetupBeforeAndAfterActions(setParent, SetParentToNull));
            return collection ?? (collection = new List<T>());
        }

        //extend

        #region 实体值字典
        /// <summary>
        /// 实体值字典
        /// </summary>
        public readonly Dictionary<string, object> DbFieldItems = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        #endregion

        #region 设置属性
        /// <summary>
        /// 设置属性
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetField(string name, object value)
        {
            DbFieldItems[name] = value;
        }
        #endregion

        #region 获取属性
        /// <summary>
        /// 获取属性
        /// </summary>
        public T GetField<T>(string name)
        {
            object value = null;

            if (DbFieldItems.TryGetValue(name, out value))
            {
                try
                {
                    return value == null || value == System.DBNull.Value ? default(T) : (T)Convert.ChangeType(value, typeof(T));
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.ToString());
                }
            }
            return default(T);
        }

        /// <summary>
        /// 获取属性(Guid需要特殊处理)
        /// </summary>
        public Guid GetFieldGuid(string name)
        {
            object value = null;

            if (DbFieldItems.TryGetValue(name, out value))
            {
                try
                {
                    return new Guid(value.ToString());
                }
                catch (Exception ex)
                {
                    if (ex.Message.ToLower().Contains("guid"))
                    {
                        return Guid.Empty;
                    }
                    else
                    {
                        throw new Exception(ex.Message);
                    }
                }
            }

            return Guid.Empty;
        }
        #endregion
    }
}