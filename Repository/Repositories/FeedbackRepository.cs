﻿using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class FeedbackRepository: IRepository<Feedback>
    {
        private readonly IContext context;
        public FeedbackRepository(IContext context)
        {
            this.context = context;
        }
        public async Task<Feedback> Add(Feedback item)
        {
            await context.Feedbacks.AddAsync(item);
            await context.Save();
            return item;
        }

        public async Task Delete(int id)
        {
            context.Feedbacks.Remove( await GetById(id));
            await context.Save();
        }

        public async Task<List<Feedback>> GetAll()
        {
            return context.Feedbacks.ToList();
        }

        public async Task<Feedback> GetById(int id)
        {
            return await context.Feedbacks.FirstOrDefaultAsync(x => x.Id == id);
        }
        //public async Task<User> GetUserFeedbackById(int feedbackId)
        //{
        //    //get feedback_id
        //    //look after feedback that its id equal to feedbackId
        //    //continue look after user that its id equal to feedback->userid
        //    Feedback feedback = await context.Feedbacks.FirstOrDefaultAsync(x => x.Id == feedbackId);
        //    User user = await context.Users.FirstOrDefaultAsync(x => x.Id == feedback.UserId);
        //    return user;
        //}

        public async Task Update(int id, Feedback item)
        {
            var existFeedback = await GetById(id);
            if (existFeedback != null)
            {
                existFeedback.Type = item.Type;
                await context.Save();
            }
        }
    }
}
