import { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { dishesApi } from '../services/api';
import type { Ingredient, DishDetails } from '../types/api';

export default function Dish() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { isAuthenticated } = useAuth();
  const isNewDish = id === 'new';

  // Show/Edit state - new dishes start in edit mode, existing dishes start in show mode
  const [isEditing, setIsEditing] = useState(isNewDish);
  const [dishDetails, setDishDetails] = useState<DishDetails | null>(null);

  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [ingredients, setIngredients] = useState<Ingredient[]>([
    { name: '', weightGrams: 0 },
  ]);
  const [imageFile, setImageFile] = useState<File | null>(null);
  const [imagePreview, setImagePreview] = useState<string | null>(null);
  const [loading, setLoading] = useState(!isNewDish);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    if (!isNewDish && id) {
      loadDish(parseInt(id));
    }
  }, [id]);

  const loadDish = async (dishId: number) => {
    try {
      setLoading(true);
      setError('');
      const dish = await dishesApi.getById(dishId);
      setDishDetails(dish);
      setName(dish.name);
      setDescription(dish.description);
      setIngredients(dish.ingredients.length > 0 ? dish.ingredients : [{ name: '', weightGrams: 0 }]);
      if (dish.hasImage) {
        setImagePreview(dishesApi.getImageUrl(dishId));
      }
    } catch (err) {
      setError('Failed to load dish');
      console.error('Error loading dish:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleEdit = () => {
    setIsEditing(true);
  };

  const handleCancelEdit = () => {
    if (isNewDish) {
      navigate('/feed');
    } else {
      // Reset form to original values
      if (dishDetails) {
        setName(dishDetails.name);
        setDescription(dishDetails.description);
        setIngredients(dishDetails.ingredients.length > 0 ? dishDetails.ingredients : [{ name: '', weightGrams: 0 }]);
        if (dishDetails.hasImage) {
          setImagePreview(dishesApi.getImageUrl(dishDetails.id));
        }
      }
      setImageFile(null);
      setError('');
      setIsEditing(false);
    }
  };

  const handleImageChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) {
      setImageFile(file);
      const reader = new FileReader();
      reader.onloadend = () => {
        setImagePreview(reader.result as string);
      };
      reader.readAsDataURL(file);
    }
  };

  const addIngredient = () => {
    setIngredients([...ingredients, { name: '', weightGrams: 0 }]);
  };

  const removeIngredient = (index: number) => {
    if (ingredients.length > 1) {
      setIngredients(ingredients.filter((_, i) => i !== index));
    }
  };

  const updateIngredient = (index: number, field: keyof Ingredient, value: string | number) => {
    const updated = [...ingredients];
    updated[index] = { ...updated[index], [field]: value };
    setIngredients(updated);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setSaving(true);

    try {
      let imageBase64: string | undefined = undefined;

      if (imageFile) {
        const reader = new FileReader();
        imageBase64 = await new Promise((resolve, reject) => {
          reader.onloadend = () => {
            const base64 = (reader.result as string).split(',')[1];
            resolve(base64);
          };
          reader.onerror = reject;
          reader.readAsDataURL(imageFile);
        });
      }

      const dishData = {
        name,
        description,
        imageBase64,
        ingredients: ingredients.filter(i => i.name && i.weightGrams > 0),
      };

      if (isNewDish) {
        await dishesApi.create(dishData);
        navigate('/feed');
      } else {
        const updatedDish = await dishesApi.update(parseInt(id!), dishData);
        setDishDetails(updatedDish);
        setIsEditing(false);
        setImageFile(null);
      }
    } catch (err) {
      setError('Failed to save dish');
      console.error('Error saving dish:', err);
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async () => {
    if (!isNewDish && id && confirm('Are you sure you want to delete this dish?')) {
      try {
        await dishesApi.delete(parseInt(id));
        navigate('/feed');
      } catch (err) {
        setError('Failed to delete dish');
        console.error('Error deleting dish:', err);
      }
    }
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-gray-900 flex items-center justify-center">
        <p className="text-gray-400">Loading...</p>
      </div>
    );
  }

  // Show state - compact view with all dish information
  if (!isEditing && dishDetails) {
    return (
      <div className="min-h-screen bg-gray-900">
        <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
          {/* Header */}
          <div className="flex justify-between items-center mb-8">
            <h1 className="text-3xl font-bold text-white">{dishDetails.name}</h1>
            <div className="flex gap-3">
              {isAuthenticated && (
                <button
                  onClick={handleEdit}
                  className="px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition-colors font-medium"
                >
                  Edit
                </button>
              )}
              <button
                onClick={() => navigate('/feed')}
                className="px-4 py-2 bg-gray-700 text-white rounded-lg hover:bg-gray-600 transition-colors"
              >
                Back to Feed
              </button>
            </div>
          </div>

          {/* Dish Content */}
          <div className="bg-gray-800 rounded-lg overflow-hidden border border-gray-700">
            {/* Image */}
            {dishDetails.hasImage && (
              <img
                src={dishesApi.getImageUrl(dishDetails.id)}
                alt={dishDetails.name}
                className="w-full h-96 object-cover"
              />
            )}
            {!dishDetails.hasImage && (
              <div className="w-full h-96 bg-gray-700 flex items-center justify-center">
                <span className="text-gray-500 text-6xl">🍽️</span>
              </div>
            )}

            {/* Details */}
            <div className="p-6">
              <div className="mb-6">
                <h2 className="text-sm font-medium text-gray-400 mb-2">Description</h2>
                <p className="text-white text-lg">{dishDetails.description}</p>
              </div>

              {/* Ingredients */}
              {dishDetails.ingredients.length > 0 && (
                <div>
                  <h2 className="text-sm font-medium text-gray-400 mb-3">Ingredients</h2>
                  <div className="bg-gray-900 rounded-lg p-4">
                    <ul className="space-y-2">
                      {dishDetails.ingredients.map((ingredient, index) => (
                        <li key={index} className="flex justify-between items-center text-white">
                          <span>{ingredient.name}</span>
                          <span className="text-gray-400">{ingredient.weightGrams}g</span>
                        </li>
                      ))}
                    </ul>
                  </div>
                </div>
              )}
            </div>
          </div>
        </div>
      </div>
    );
  }

  // Edit state - full form
  return (
    <div className="min-h-screen bg-gray-900">
      <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Header */}
        <div className="flex justify-between items-center mb-8">
          <h1 className="text-3xl font-bold text-white">
            {isNewDish ? 'Create New Dish' : 'Edit Dish'}
          </h1>
          <button
            onClick={() => navigate('/feed')}
            className="px-4 py-2 bg-gray-700 text-white rounded-lg hover:bg-gray-600 transition-colors"
          >
            Back to Feed
          </button>
        </div>

        {/* Error Message */}
        {error && (
          <div className="mb-6 p-4 bg-red-900 border border-red-700 rounded-lg">
            <p className="text-red-200">{error}</p>
          </div>
        )}

        {/* Form */}
        <form onSubmit={handleSubmit} className="space-y-6">
          {/* Image Upload */}
          <div>
            <label className="block text-sm font-medium text-gray-300 mb-2">
              Dish Image
            </label>
            {imagePreview && (
              <div className="mb-4">
                <img
                  src={imagePreview}
                  alt="Preview"
                  className="w-full max-w-md h-64 object-cover rounded-lg"
                />
              </div>
            )}
            <input
              type="file"
              accept="image/*"
              onChange={handleImageChange}
              className="block w-full text-sm text-gray-400
                file:mr-4 file:py-2 file:px-4
                file:rounded-lg file:border-0
                file:text-sm file:font-semibold
                file:bg-indigo-600 file:text-white
                hover:file:bg-indigo-700 file:cursor-pointer"
            />
          </div>

          {/* Name */}
          <div>
            <label htmlFor="name" className="block text-sm font-medium text-gray-300 mb-2">
              Dish Name *
            </label>
            <input
              id="name"
              type="text"
              required
              maxLength={200}
              className="w-full px-4 py-2 bg-gray-800 border border-gray-700 rounded-lg text-white placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-indigo-500"
              value={name}
              onChange={(e) => setName(e.target.value)}
            />
          </div>

          {/* Description */}
          <div>
            <label htmlFor="description" className="block text-sm font-medium text-gray-300 mb-2">
              Description *
            </label>
            <textarea
              id="description"
              required
              maxLength={2000}
              rows={4}
              className="w-full px-4 py-2 bg-gray-800 border border-gray-700 rounded-lg text-white placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-indigo-500"
              value={description}
              onChange={(e) => setDescription(e.target.value)}
            />
          </div>

          {/* Ingredients */}
          <div>
            <div className="flex justify-between items-center mb-2">
              <label className="block text-sm font-medium text-gray-300">
                Ingredients *
              </label>
              <button
                type="button"
                onClick={addIngredient}
                className="px-3 py-1 bg-indigo-600 text-white text-sm rounded hover:bg-indigo-700 transition-colors"
              >
                + Add Ingredient
              </button>
            </div>
            <div className="space-y-3">
              {ingredients.map((ingredient, index) => (
                <div key={index} className="flex gap-3">
                  <input
                    type="text"
                    placeholder="Ingredient name"
                    required
                    maxLength={200}
                    className="flex-1 px-4 py-2 bg-gray-800 border border-gray-700 rounded-lg text-white placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-indigo-500"
                    value={ingredient.name}
                    onChange={(e) => updateIngredient(index, 'name', e.target.value)}
                  />
                  <input
                    type="number"
                    placeholder="Weight (g)"
                    required
                    min="0.01"
                    step="0.01"
                    className="w-32 px-4 py-2 bg-gray-800 border border-gray-700 rounded-lg text-white placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-indigo-500"
                    value={ingredient.weightGrams || ''}
                    onChange={(e) => updateIngredient(index, 'weightGrams', parseFloat(e.target.value) || 0)}
                  />
                  {ingredients.length > 1 && (
                    <button
                      type="button"
                      onClick={() => removeIngredient(index)}
                      className="px-3 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 transition-colors"
                    >
                      Remove
                    </button>
                  )}
                </div>
              ))}
            </div>
          </div>

          {/* Actions */}
          <div className="flex justify-between pt-6">
            <div>
              {!isNewDish && (
                <button
                  type="button"
                  onClick={handleDelete}
                  className="px-6 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 transition-colors font-medium"
                >
                  Delete Dish
                </button>
              )}
            </div>
            <div className="flex gap-3">
              <button
                type="button"
                onClick={handleCancelEdit}
                className="px-6 py-2 bg-gray-700 text-white rounded-lg hover:bg-gray-600 transition-colors"
              >
                Cancel
              </button>
              <button
                type="submit"
                disabled={saving}
                className="px-6 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors font-medium"
              >
                {saving ? 'Saving...' : isNewDish ? 'Create Dish' : 'Save Changes'}
              </button>
            </div>
          </div>
        </form>
      </div>
    </div>
  );
}
