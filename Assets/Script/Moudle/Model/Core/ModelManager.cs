﻿using System;
using System.Collections.Generic;
using Common.Tool;

public class ModelManager :Singleton<ModelManager>
{
    private Dictionary<Type, ModelBase> m_ModelStore;
    public ModelManager()
    {
        CheckInit();
    }
    private void AutoRegister()
    {
        var list = ReflectionManager.Instance.GetTypeByBase(typeof (ModelBase));
        for (int i = 0; i < list.Count; ++i)
        {
            var elem = list[i];
            ModelBase modelInstance = Activator.CreateInstance(elem) as ModelBase;
            modelInstance.OnCreate();
            m_ModelStore.Add(elem, modelInstance);
        }
    }
    public T GetModel<T>() where T : ModelBase
    {
        ModelBase res = null;
        m_ModelStore.TryGetValue(typeof (T), out res);
        return res as T;
    }
    public void CheckInit()
    {
        m_ModelStore = new Dictionary<Type, ModelBase>();
        AutoRegister();

    }
}
