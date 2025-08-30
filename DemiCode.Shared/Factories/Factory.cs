using System;
using System.Globalization;
using System.Reflection;
using System.Xml;

namespace DemiCode.Shared.Factories
{
	/// <summary>
	/// Methods to dynamically create objects of different types from XML
	/// </summary>
	internal static class Factory
    {
        #region Factory improvements

        // TODO: Factory improvements
        //Create instances directly from an XmlReader in addition to XmlNode.

        //#region static ctor
        //static Factory()
        //{
        //    // Create list of assemblies (in memory) that are ok, and instantiate only from them. NEVER load from
        //    // unchecked assemblies...

        //    //AppDomain.CreateDomain(...);
        //    //load specified assembly into the new appdomain
        //    //check for interfaces (and also possibly other evidence) to prove this is a valid assembly
        //}
        //#endregion

		//Before instantiating ANY dynamic classes: Check them in a seperate AppDomain...

        //Load assemblies from SourceStreamProviders???
        
        #endregion

        /// <summary>
        /// Creates a new instance of a the type described in config's "type" attribute. That type must implement interface <typeparamref name="T"/>.
		/// </summary>
		/// <remarks>
		/// The fully qualified typename includes the name of the types containing assembly without
		/// the filename extension. (Like "namespaces.classname, assemblyname".)
		/// </remarks>
		/// <typeparam name="T">An interface supported of typeToCreate</typeparam>
		/// <param name="config">The XmlNode containing a "type" attribute to describe what type to create. This inNode is also passed to the created type's constructor</param>
		/// <returns>A interface reference to an object of type typeToCreate</returns>
		static public T CreateInstance<T>(XmlNode config)
			where T : class
		{
            XmlAttribute typeAttribute = config.Attributes.GetNamedItem("type") as XmlAttribute;
            if (typeAttribute != null)
            {
                string typeName = typeAttribute.Value;
                if (typeName != null)
                {
                    return CreateInstance<T>(typeName, config);
                }
            }

			string message = "Supplied parameter 'config' has no 'type' attribute";
			//MessageLogger.LogError(message);
			throw new FactoryConfigurationException(message);
		}

		/// <summary>
        /// Creates a new instance of typeName which implements interface <typeparamref name="T"/> or inherits from a base type <typeparamref name="T"/>.
		/// </summary>
		/// <remarks>
		/// The fully qualified typename includes the name of the types containing assembly without
		/// the filename extension. (Like "namespaces.classname, assemblyname".)
		/// </remarks>
		/// <typeparam name="T">An interface supported of typeName</typeparam>
		/// <param name="typeName">The fully qualified name of the type to instantiate</param>
		/// <returns>A interface reference to an object of type typeName</returns>
		static public T CreateInstance<T>(string typeName)
			where T : class
		{
		    if (String.IsNullOrEmpty(typeName))
		        throw new ArgumentNullException("typeName");
 
			Type t = Type.GetType(typeName, false);	//~ArgumentNullException, TargetInvocationException, TypeLoadException
			if (t == null)
			{
				string message = String.Format(CultureInfo.InvariantCulture, "Type '{0}' not found", typeName);
				//MessageLogger.LogError(message);
				throw new FactoryException(message);
			}

			return CreateInstance<T>(t);
		}

        /// <summary>
        /// Creates a new instance of typeName which implements interface <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>
        /// The fully qualified typename includes the name of the types containing assembly without
        /// the filename extension. (Like "namespaces.classname, assemblyname".)
        /// </remarks>
        /// <typeparam name="T">An interface supported of typeName</typeparam>
        /// <param name="typeName">The fully qualified name of the type to instantiate</param>
        /// <param name="config">The XML inNode to be passed to typeName's constructor</param>
        /// <returns>A interface reference to an object of type typeName</returns>
        static public T CreateInstance<T>(string typeName, XmlNode config)
            where T : class
        {
            Type t = Type.GetType(typeName, false);	//~ArgumentNullException, TargetInvocationException, TypeLoadException
            if (t == null)
            {
                string message = String.Format(CultureInfo.InvariantCulture, "Type '{0}' not found", typeName);
                //MessageLogger.LogError(message);
                throw new FactoryException(message);
            }

            return CreateInstance<T>(t, config);
        }

        /// <summary>
        /// Creates a new instance of typeToCreate
        /// </summary>
        /// <typeparam name="T">An interface supported of typeToCreate</typeparam>
        /// <param name="typeToCreate">The type to instantiate</param>
        /// <returns>A interface reference to an object of type typeToCreate</returns>
		static public T CreateInstance<T>(Type typeToCreate)
			where T : class
		{
            if (typeToCreate == null)
                throw new ArgumentNullException("typeToCreate", "The value of typeToCreate cannot be null");

            return CreateInstanceInternal<T>(typeToCreate, null);
        }

		/// <summary>
        /// Creates a new instance of typeToCreate which implements interface <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T">An interface supported of typeToCreate</typeparam>
		/// <param name="typeToCreate">The type to instantiate</param>
		/// <param name="config">The XML inNode to be passed to typeToCreate's constructor</param>
		/// <returns>A interface reference to an object of type typeToCreate</returns>
		static public T CreateInstance<T>(Type typeToCreate, XmlNode config)
			where T : class
		{
            if (typeToCreate == null)
                throw new ArgumentNullException("typeToCreate", "The value of typeToCreate cannot be null");

            if (config == null)
                throw new ArgumentNullException("config", "The value of config cannot be null");

            return CreateInstanceInternal<T>(typeToCreate, config);
		}

        public static T CreateInstance<T>(Type typeToCreate, params object[] constructorParameters)
            where T : class
        {
            return CreateInstanceInternalWithArgs<T>(typeToCreate, constructorParameters);
        }

        private static T CreateInstanceInternal<T>(Type typeToCreate, XmlNode config)
            where T : class
        {
	        try
	        {
	            //Get the constructor that takes the requested parameters
	            ConstructorInfo ci;
                if (config != null)
                {
                    ci = typeToCreate.GetConstructor(new Type[] { typeof(XmlNode) });	//~ArgumentNullException, ArgumentException
                }
                else
                {
                    ci = typeToCreate.GetConstructor(new Type[0]);	//~ArgumentNullException, ArgumentException
                }
	            //Check if we got one
	            if (ci == null)
	            {
	                throw new FactoryException(String.Format(CultureInfo.InvariantCulture, "Type '{0}' has no constructors that accept the requested parameters", typeToCreate.Name));
	            }

	            T createdType; 
                if (config != null)
                {
                    createdType = ci.Invoke(new object[] { config }) as T;	//MethodAccessException, ~ArgumentException, TargetInvocationException, TargetParameterCountException
                }
                else
                {
                    createdType = ci.Invoke(null) as T;	//MethodAccessException, ~ArgumentException, TargetInvocationException, TargetParameterCountException
                }
	            if (createdType == null)
	            {
	                //TODO: Check if typeof(T).GetType().Name returns the word 'Runtimetype' or the actual name of the interface
	                throw new FactoryException(String.Format(CultureInfo.InvariantCulture, "Type '{0}' doesn't support the '{1}' interface or base type", typeToCreate.Name, typeof(T).GetType().Name));
	            }

	            //MessageLogger.LogInfo("Successfully created object of type '{0}'", typeToCreate.Name);

	            return createdType;
	        }
	        catch (FactoryException e)
	        {
	            //MessageLogger.LogError(e.Message);
	            throw;
	        }
	        catch (TargetInvocationException e)
	        {
	            string additionalMessage = null;
	            if (e.InnerException != null)
	                additionalMessage = String.Format(CultureInfo.InvariantCulture, "{0}.Message: '{1}'", e.InnerException.GetType().Name, e.InnerException.Message);
	            string message = String.Format(CultureInfo.InvariantCulture, "Unable to create object of type '{0}'. {1}", typeToCreate.Name, additionalMessage);
                
	            //MessageLogger.LogError(message);
	            throw new FactoryException(message, e);
	        }
	        catch (Exception e)	//TypeLoadException, ArgumentException, MethodAccessException, TargetParameterCountException
	        {
	            string message = String.Format(CultureInfo.InvariantCulture, "Unable to create object of type '{0}'. {1} saying '{2}'.", typeToCreate.Name, e.GetType().Name, e.Message);

	            //MessageLogger.LogError(message);
	            throw new FactoryException(message, e);
	        }
	    }

        private static T CreateInstanceInternalWithArgs<T>(Type typeToCreate, params object[] constructorParameters)
        where T : class
        {
            try
            {
                //Get the constructor that takes the requested parameters
                ConstructorInfo ci;
                Type[] paramTypes = new Type[constructorParameters.Length];
                for (int i = 0; i < constructorParameters.Length; i++ )
                {
                    paramTypes[i] = constructorParameters[i].GetType();
                }
                ci = typeToCreate.GetConstructor(paramTypes);	

                //Check if we got a matching constructor
                if (ci == null)
                {
                    throw new FactoryException(String.Format(CultureInfo.InvariantCulture, "Type '{0}' has no constructors that accept the requested parameters", typeToCreate.Name));
                }

                T createdType;
                if (constructorParameters.Length > 0)
                {
                    createdType = ci.Invoke(constructorParameters) as T;	//MethodAccessException, ~ArgumentException, TargetInvocationException, TargetParameterCountException
                }
                else
                {
                    createdType = ci.Invoke(null) as T;	//MethodAccessException, ~ArgumentException, TargetInvocationException, TargetParameterCountException
                }
                if (createdType == null)
                {
                    //TODO: Check if typeof(T).GetType().Name returns the word 'Runtimetype' or the actual name of the interface
                    throw new FactoryException(String.Format(CultureInfo.InvariantCulture, "Type '{0}' doesn't support the '{1}' interface or base type", typeToCreate.Name, typeof(T).GetType().Name));
                }

                // MessageLogger.LogInfo("Successfully created object of type '{0}'", typeToCreate.Name);

                return createdType;
            }
            catch (FactoryException e)
            {
                //MessageLogger.LogError(e.Message);
                throw;
            }
            catch (TargetInvocationException e)
            {
                string additionalMessage = null;
                if (e.InnerException != null)
                    additionalMessage = String.Format(CultureInfo.InvariantCulture, "{0}.Message: '{1}'", e.InnerException.GetType().Name, e.InnerException.Message);
                string message = String.Format(CultureInfo.InvariantCulture, "Unable to create object of type '{0}'. {1}", typeToCreate.Name, additionalMessage);

                //MessageLogger.LogError(message);
                throw new FactoryException(message, e);
            }
            catch (Exception e)	//TypeLoadException, ArgumentException, MethodAccessException, TargetParameterCountException
            {
                string message = String.Format(CultureInfo.InvariantCulture, "Unable to create object of type '{0}'. {1} saying '{2}'.", typeToCreate.Name, e.GetType().Name, e.Message);

                //MessageLogger.LogError(message);
                throw new FactoryException(message, e);
            }
        }
    }
}
